using System;
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
        private object Resolve(Type serviceType)
        {
            // Find service maching given type
            var descriptor = _serviceDescriptors
                .SingleOrDefault(x => x.ServiceType == serviceType);

            // If the type is not found thrown an error
            if (descriptor == null)
                throw new Exception($"Service of type {serviceType} is not registered");

            // If there is an implemetation return it
            if (descriptor.Implementation != null)
                return descriptor.Implementation;

            // Get the type needed to instantiate
            var initType = descriptor.ImplementationType;

            // Throw exception if type is abstract or an interface 
            if (initType.IsAbstract || initType.IsInterface)
                throw new Exception($"Cannot instantiate " +
                    $"{(initType.IsAbstract ? "abstract class." : "interface.")}");

            // Get the first constructor for the type
            // This will clash if the type has multiple contrustors :(
            var contructorInfo = initType.GetConstructors().First();
            
            // Use recursion to instantiate contructor parameters
            var parameters = contructorInfo
                .GetParameters()
                .Select(x => Resolve(x.ParameterType))
                .ToArray();

            // Instatiate type
            var implementation = Activator.CreateInstance(initType, parameters);

            // If the lifecycle is singleton set the implementation
            if (descriptor.Lifetime == ELifecycleType.SINGLETON)
                descriptor.Implementation = implementation;

            return implementation;
        }

        /// <summary>
        /// Resolves a service given a generic type.
        /// Service must be registered to properly resolve.
        /// </summary>
        /// <typeparam name="TService">Type to resolve implementation</typeparam>
        /// <returns>TService implementation</returns>
        public TService Resolve<TService>() => (TService)Resolve(typeof(TService));
    }
}
