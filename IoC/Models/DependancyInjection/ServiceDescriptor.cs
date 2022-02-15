using System;
namespace IoC.Models.DependancyInjection
{
    /// <summary>
    /// Describes the service and its associated settings
    /// </summary>
    public class ServiceDescriptor
    {
        public Type ServiceType { get; }
        public Type ImplementationType { get; }
        public object Implementation { get; internal set; }
        public ELifecycleType Lifetime { get; } = ELifecycleType.TRANSIENT;

        public ServiceDescriptor(Type serviceType, Type implementationType, ELifecycleType lifetime)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
        }
    }
}
