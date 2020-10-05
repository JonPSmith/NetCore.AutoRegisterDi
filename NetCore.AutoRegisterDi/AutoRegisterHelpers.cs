// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.
// Code added/updated by Fedor Zhekov, GitHub: @ZFi88

using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace NetCore.AutoRegisterDi
{
    /// <summary>
    /// This contains the extension methods for registering classes automatically
    /// </summary>
    public static class AutoRegisterHelpers
    {
        /// <summary>
        /// This finds all the public, non-generic, non-nested classes in an assembly in the provided assemblies.
        /// If no assemblies provided then it scans the assembly that called the method
        /// </summary>
        /// <param name="services">the NET Core dependency injection service</param>
        /// <param name="assemblies">Each assembly you want scanned. If null then scans the the assembly that called the method</param>
        /// <returns></returns>
        public static AutoRegisterData RegisterAssemblyPublicNonGenericClasses(this IServiceCollection services,
            params Assembly[] assemblies)
        {
            if (assemblies.Length == 0)
                assemblies = new[] {Assembly.GetCallingAssembly()};

            var allPublicTypes = assemblies.SelectMany(x => x.GetExportedTypes()
                .Where(y => y.IsClass && !y.IsAbstract && !y.IsGenericType && !y.IsNested && !y.IsIgnoredType()));
            return new AutoRegisterData(services, allPublicTypes);
        }

        /// <summary>
        /// This allows you to filter the classes in some way.
        /// For instance <code>Where(c =\> c.Name.EndsWith("Service")</code> would only register classes who's name ended in "Service"
        /// </summary>
        /// <param name="autoRegData"></param>
        /// <param name="predicate">A function that will take a type and return true if that type should be included</param>
        /// <returns></returns>
        public static AutoRegisterData Where(this AutoRegisterData autoRegData, Func<Type, bool> predicate)
        {
            if (autoRegData == null) throw new ArgumentNullException(nameof(autoRegData));
            return new AutoRegisterData(autoRegData.Services, autoRegData.TypesToConsider.Where(predicate));
        }

        /// <summary>
        /// This allows you to state that the given interface will not be registered against a class.
        /// Useful if a class has a interface that you don't want registered against a class.
        /// NOTE: the <see cref="IDisposable"/> and <see cref="ISerializable"/> interfaces are automatically ignored
        /// </summary>
        /// <typeparam name="TInterface">interface to be ignored</typeparam>
        /// <param name="autoRegData"></param>
        /// <returns></returns>
        public static AutoRegisterData IgnoreThisInterface<TInterface>(this AutoRegisterData autoRegData)
        {
            if (!typeof(TInterface).IsInterface)
                throw new InvalidOperationException($"The provided {typeof(TInterface).Name} mus be an interface");
            autoRegData.InterfacesToIgnore.Add(typeof(TInterface));
            return autoRegData;
        }

        /// <summary>
        /// This registers the classes against any public interfaces (other than InterfacesToIgnore) implemented by the class
        /// </summary>
        /// <param name="autoRegData">AutoRegister data produced by <see cref="RegisterAssemblyPublicNonGenericClasses"/></param> method
        /// <param name="lifetime">Allows you to define the lifetime of the service - defaults to ServiceLifetime.Transient</param>
        /// <returns></returns>
        public static IServiceCollection AsPublicImplementedInterfaces(this AutoRegisterData autoRegData,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            if (autoRegData == null) throw new ArgumentNullException(nameof(autoRegData));
            foreach (var classType in autoRegData.TypesToConsider)
            {
                if (classType.IsMultipleLifetime())
                    throw new ArgumentException($"Class {classType.FullName} has multiple life time attributes");

                var interfaces = classType.GetTypeInfo().ImplementedInterfaces;
                foreach (var infc in interfaces.Where(i => 
                    !autoRegData.InterfacesToIgnore.Contains(i) //This will not register the class with this interface
                    && i.IsPublic && !i.IsNested))
                {
                    autoRegData.Services.Add(new ServiceDescriptor(infc, classType, classType.GetLifetimeForClass(lifetime)));
                }
            }

            return autoRegData.Services;
        }
    }
}