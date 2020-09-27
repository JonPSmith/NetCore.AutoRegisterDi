// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.
// Code added/updated by Fedor Zhekov, GitHub: @ZFi88

using System.Runtime.CompilerServices;
using NetCore.AutoRegisterDi.PublicButHidden;

[assembly: InternalsVisibleTo("Test")]

namespace NetCore.AutoRegisterDi
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;


    /// <summary>
    /// Extensions for <see cref="Type"/>
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        ///     Check if type is marked by <see cref="DoNotAutoRegisterAttribute" /> or if type is an exception and not marked with
        ///     any <see cref="RegisterWithLifetimeAttribute" />.
        /// </summary>
        /// <param name="type">type</param>
        public static bool IsIgnoredType(this Type type)
        {
            var doNotAutoRegisterAttribute = type.GetCustomAttribute<DoNotAutoRegisterAttribute>();
            var isManuallyExcluded = doNotAutoRegisterAttribute != null;

            if (isManuallyExcluded)
            {
                return true;
            }

            var isException = type.IsSubclassOf(typeof(Exception));
            return isException && type.GetCustomAttribute<RegisterWithLifetimeAttribute>() == null;
        }

        /// <summary>
        /// Check if class marked by multiple lifetime attributes
        /// </summary>
        /// <param name="type">type</param>
        /// <returns></returns>
        public static bool IsMultipleLifetime(this Type type)
        {
            return type.GetCustomAttributes(typeof(RegisterWithLifetimeAttribute), true).Count() > 1;
        }

        /// <summary>
        /// Returns service lifetime
        /// </summary>
        /// <param name="type">type</param>
        /// <param name="lifetime">If no attribute then it returns the lifetime provided in the AsPublicImplementedInterfaces parameter</param>
        /// <returns></returns>
        public static ServiceLifetime GetLifetimeForClass(this Type type, ServiceLifetime lifetime)
        {
            return type.GetCustomAttribute<RegisterWithLifetimeAttribute>(true)?.RequiredLifetime ?? lifetime;
        }
    }
}
