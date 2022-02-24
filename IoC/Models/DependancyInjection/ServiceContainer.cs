using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IoC.Models.DependancyInjection
{
    /// <summary>
    /// Dependancy injection service collection
    /// </summary>
    public class ServiceContainer
    {
        /// <summary>
        /// List of services registered to the collection
        /// </summary>
        private List<ServiceDescriptor> _serviceDescriptors = new List<ServiceDescriptor>();

        public ServiceContainer() {}

        /// <summary>
        /// Registers a service to the collection.
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <typeparam name="TImplementation">Service implementation</typeparam>
        /// <param name="lifetime">Service registration lifecycle - default is transient</param>
        public void Register<TService, TImplementation>(ELifecycleType lifetime = ELifecycleType.TRANSIENT) where TImplementation : TService
        {
            _serviceDescriptors.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
        }

        /// <summary>
        /// Resolves the first found implementation for the give type
        /// If exists in the collection.
        /// </summary>
        /// <param name="serviceType">Type to resolve an implementation</param>
        /// <returns>Implementation for serviceType</returns>
        private dynamic Resolve(Type serviceType)
        {
            // Find service maching given type
            var descriptors = _serviceDescriptors
                .Where(x => x.ServiceType == serviceType)
                .ToList();

            // If the type is not found thrown an error
            if (descriptors.Count == 0) throw new Exception($"Service of type {serviceType} is not registered");

            // Create a generic list of the requested type
            Type list_class = typeof(List<>).MakeGenericType(serviceType);
            var implementations = (IList)Activator.CreateInstance(list_class);

            foreach (var descriptor in descriptors) {
                // If there is an implemetation add to list and break
                if (descriptor.Implementation != null)
                {
                    implementations.Add(descriptor.Implementation);
                    break;
                }

                // Get the type needed to instantiate
                var initType = descriptor.ImplementationType;
                
                // Throw exception if type is abstract or an interface 
                if (initType.IsAbstract || initType.IsInterface)
                    throw new Exception($"Cannot instantiate " +
                        $"{(initType.IsAbstract ? "abstract class." : "interface.")}");

                // Get the first constructor for the type
                // This will clash if the type has multiple contructors :(
                var contructorInfo = initType.GetConstructors().First();

                // Use recursion to instantiate contructor parameters
                // This can create a bug if specific instances of a service are needed as the injection will resolve to the first item found
                var parameters = contructorInfo
                    .GetParameters()
                    .Select(x => {
                        var resolution = Resolve(x.ParameterType);
                        if (resolution is IList) return (IList)resolution[0];
                        return resolution;
                    })
                    .ToArray();

                // Instatiate type
                var implementation = Activator.CreateInstance(initType, parameters);

                // If the lifecycle is singleton set the implementation
                if (descriptor.Lifetime == ELifecycleType.SINGLETON)
                    descriptor.Implementation = implementation;

                implementations.Add(implementation);
            }

            // If there is only one registered implementation of the serviceType return it.
            if (implementations.Count == 1) return implementations[0];

            // Return all registered implemetations of the serviceType
            return implementations;
        }

        /// <summary>
        /// Resolves a service given a generic type.
        /// Service must be registered to properly resolve.
        /// </summary>
        /// <typeparam name="TService">Type to resolve implementation</typeparam>
        /// <returns>TService implementation</returns>
        public dynamic Resolve<TService>() => Resolve(typeof(TService));
    }
}
