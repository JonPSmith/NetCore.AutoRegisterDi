using System;
using Microsoft.Extensions.DependencyInjection;

namespace NetCore.AutoRegisterDi
{
    /// <summary>
    /// This contains the registered class with its interface and lifetime that were register wit DI
    /// If the <see cref="Class"/> is null, then it is listing the ignored interfaces 
    /// </summary>
    public class AutoRegisteredResult
    {
        internal AutoRegisteredResult(Type classType, Type interfaceType, ServiceLifetime lifetime)
        {
            Class = classType;
            Interface = interfaceType;
            Lifetime = lifetime;
        }

        /// <summary>
        /// If not null, then it contains the class that is registered with DI
        /// If null, then the class is showing an interface in the ignored list
        /// </summary>
        public Type Class { get; }

        /// <summary>
        /// If Class is not null, then this is the interface that the class was registered against
        /// If Class is  null, then the interface is ignored
        /// </summary>
        public Type Interface { get; }

        /// <summary>
        /// If Class is not null this contains the ServiceLifetime used when the class was registered in DI
        /// </summary>
        public ServiceLifetime Lifetime { get; }

        /// <summary>
        /// Gives a easy to read summary of a result.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Class == null
                ? $"The interface {Interface.Name} is ignored"
                : $"{Class.Name} : {Interface.Name} ({Lifetime})";
        }
    }
}