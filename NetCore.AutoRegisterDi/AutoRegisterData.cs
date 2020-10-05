// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace NetCore.AutoRegisterDi
{
    /// <summary>
    /// This holds the data passed between the the various Stages
    /// </summary>
    public class AutoRegisterData
    {
        /// <summary>
        /// RegisterAssemblyPublicNonGenericClasses uses this to create the initial data
        /// </summary>
        /// <param name="services"></param>
        /// <param name="typesToConsider"></param>
        public AutoRegisterData(IServiceCollection services, IEnumerable<Type> typesToConsider)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            TypesToConsider = typesToConsider ?? throw new ArgumentNullException(nameof(typesToConsider));
        }

        /// <summary>
        /// This carries the service register 
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// This holds the class types found by the RegisterAssemblyPublicNonGenericClasses
        /// </summary>
        public IEnumerable<Type> TypesToConsider { get; }

        /// <summary>
        /// 
        /// </summary>
        internal List<Type> InterfacesToIgnore { get; set; }
            = new List<Type>
            {
                typeof(IDisposable),
                typeof(ISerializable)
            };
    }
}