// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace NetCore.AutoRegisterDi.PublicButHidden
{
    /// <summary>
    /// Attribute for marking classes with the ServiceLifetime they should be registered as
    /// NOTE: We don't expect to use this attribute as there are better named attributes to use
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterWithLifetimeAttribute : Attribute
    {
        /// <summary>
        /// This sets the ServiceLifetime that the AsPublicImplementedInterfaces method should use for this class
        /// </summary>
        /// <param name="requiredLifetime"></param>
        public RegisterWithLifetimeAttribute(ServiceLifetime requiredLifetime)
        {
            RequiredLifetime = requiredLifetime;
        }

        /// <summary>
        /// This holds the ServiceLifetime that the class which has this attribute should be registered as 
        /// </summary>
        public ServiceLifetime RequiredLifetime { get; }
    }
}