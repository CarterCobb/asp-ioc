using System;
namespace IoC.Models.DependancyInjection
{
    /// <summary>
    /// Service lifecycle options
    /// </summary>
    public enum ELifecycleType
    {
        /// <summary>
        /// Single instance throughout execution.
        /// </summary>
        SINGLETON,
        /// <summary>
        /// New instance per request during execution.
        /// </summary>
        TRANSIENT
    }
}
