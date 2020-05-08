using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Test")]

namespace NetCore.AutoRegisterDi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Attributes;
    using Microsoft.Extensions.DependencyInjection;


    /// <summary>
    /// Extensions for <see cref="Type"/>
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// Check if type marked by <see cref="DoNotAutoRegisterAttribute"/>
        /// </summary>
        /// <param name="type">type</param>
        public static bool IsIgnoredType(this Type type)
        {
            var doNotAutoRegisterAttribute = type.GetCustomAttribute<DoNotAutoRegisterAttribute>();
            return doNotAutoRegisterAttribute != null;
        }

        /// <summary>
        /// Check if class marked by multiple lifetime attributes
        /// </summary>
        /// <param name="type">type</param>
        /// <returns></returns>
        public static bool IsMultipleLifetime(this Type type)
        {
            var attributes = type.GetCustomAttributes(true)
                .Select(x => x.GetType())
                .ToList();
            if (attributes.Any())
            {
                return new List<bool>
                {
                    attributes.Contains(typeof(RegisterAsTransientAttribute)),
                    attributes.Contains(typeof(RegisterAsScopedAttribute)),
                    attributes.Contains(typeof(RegisterAsSingletonAttribute))
                }.Count(x => x) > 1;
            }

            return false;
        }

        /// <summary>
        /// Returns service lifetime
        /// </summary>
        /// <param name="type">type</param>
        /// <returns></returns>
        public static ServiceLifetime GetTypeLiteTime(this Type type)
        {
            if (type.GetCustomAttribute<RegisterAsTransientAttribute>() != null)
            {
                return ServiceLifetime.Transient;
            }

            if (type.GetCustomAttribute<RegisterAsScopedAttribute>() != null)
            {
                return ServiceLifetime.Scoped;
            }

            if (type.GetCustomAttribute<RegisterAsSingletonAttribute>() != null)
            {
                return ServiceLifetime.Singleton;
            }

            return ServiceLifetime.Transient;
        }
    }
}