// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        internal AutoRegisterData(IServiceCollection services, IEnumerable<Type> typesToConsider)
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
                typeof(ISerializable),
                typeof(IEquatable<>) //added for records
            };


        internal List<Type> FullyDefinedInterfacesToIgnore { get; set; }
            = new List<Type>
            {
                typeof(IDisposable),
                typeof(ISerializable)
            };

        internal List<Type> NotFullyDefinedInterfacesToIgnore { get; set; }
            = new List<Type>
            {
                //typeof(IEquatable<>)
            };

        /// <summary>
        /// This returns the Interfaces that will be ignored when registering the classes 
        /// </summary>
        /// <returns></returns>
        public List<Type> InterfacesIgnored()
        {
            FullyDefinedInterfacesToIgnore.AddRange(NotFullyDefinedInterfacesToIgnore);
            return FullyDefinedInterfacesToIgnore;
        }

        /// <summary>
        /// This returns the interfaces that the class will be registered to the DI provider.
        /// - It only looks at interfaces that are public and not nested
        /// - Then it uses the two ignore interface lists
        ///    - Removes a interface that exactly matches the class's interface
        ///    - Removes generic interface that by its name, eg. IList{}
        /// </summary>
        /// <param name="classType">This class is tested to find any  </param>
        /// <returns></returns>
        internal IEnumerable<Type> AllInterfacesForThisType(Type classType)
        {
            return classType.GetTypeInfo().ImplementedInterfaces
                .Where(interfaceType => interfaceType.IsPublic && !interfaceType.IsNested 
                    //This will not register the class with this interface
                    && !FullyDefinedInterfacesToIgnore.Contains(interfaceType)
                    //This will not register the class where a generic type with any type arguments 
                    //adding the type `IList<>` would stop any `IList<>`, eg. `IList<int>`, `IList<string>`, `IList<???>` etc.
                    && NotFullyDefinedInterfacesToIgnore.All(x => x.Name != interfaceType.Name)); //will not register the class
        }
    }
}