// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.
// Code added/updated by Fedor Zhekov, GitHub: @ZFi88

using System;
using System.Collections.Generic;
using System.Linq;
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
        /// NOTE: You can have multiple calls to this method to apply a series off filters
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
        /// Useful if a class has an interface that you don't want registered against a class
        ///  fully defined interfaces, e.g. MyInterface, IList{MyClass}, to be ignored
        /// NOTE: the <see cref="IDisposable"/> and <see cref="ISerializable"/> interfaces are automatically ignored
        /// </summary>
        /// <typeparam name="TInterface">interface to be ignored</typeparam>
        /// <param name="autoRegData"></param>
        /// <returns></returns>
        public static AutoRegisterData IgnoreThisInterface<TInterface>(this AutoRegisterData autoRegData)
        {
            var interfaceType = typeof(TInterface);
            if (!interfaceType.IsInterface)
                throw new InvalidOperationException($"The provided {interfaceType.Name} must be an interface.");

            autoRegData.FullyDefinedInterfacesToIgnore.Add(interfaceType);
            return autoRegData;
        }

        /// <summary>
        /// This allows you to state that the given interface will not be registered against a class.
        /// Useful if a class has an interface that you don't want registered against a class. 
        /// This method handles generic interface where the inner generic type arguments aren't defined, which will stop all interfaces
        /// that uses the main type, e.g. IList{} would stop IList{int}, IList{string}, IList{AnotherClass}, etc.
        /// </summary>
        /// <param name="autoRegData"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static AutoRegisterData IgnoreThisGenericInterface(this AutoRegisterData autoRegData, Type interfaceType)
        {
            if (!interfaceType.IsInterface)
                throw new InvalidOperationException($"The provided {interfaceType.Name} must be an interface.");
            if (interfaceType.IsGenericType && !interfaceType.GenericTypeArguments.Any())
            {
                //This is Generic Type and it hasn't defined the inner generic type arguments, e.g. List<>
                autoRegData.NotFullyDefinedInterfacesToIgnore.Add(interfaceType);
            }
            else
            {
                throw new InvalidOperationException($"The provided {interfaceType.Name} isn't a generic type or it is fully defined.");
            }
            return autoRegData;
        }


        /// <summary>
        /// This registers the classes against any public interfaces (other than InterfacesToIgnore) implemented by the class
        /// </summary>
        /// <param name="autoRegData">AutoRegister data produced by <see cref="RegisterAssemblyPublicNonGenericClasses"/></param> method
        /// <param name="lifetime">Allows you to define the lifetime of the service - defaults to ServiceLifetime.Transient</param>
        /// <returns></returns>
        public static IList<AutoRegisteredResult> AsPublicImplementedInterfaces(this AutoRegisterData autoRegData,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            if (autoRegData == null) throw new ArgumentNullException(nameof(autoRegData));

            //This lists all the ignored interfaces
            var result = autoRegData.InterfacesIgnored().Select(x =>
                new AutoRegisteredResult(null, x, ServiceLifetime.Singleton))
                .ToList();

            foreach (var classType in autoRegData.TypesToConsider)
            {
                if (classType.IsMultipleLifetime())
                    throw new ArgumentException($"Class {classType.FullName} has multiple life time attributes");

                var interfaces = autoRegData.AllInterfacesForThisType(classType);
                foreach (var infc in interfaces)
                {
                    var lifetimeForClass = classType.GetLifetimeForClass(lifetime);
                    autoRegData.Services.Add(new ServiceDescriptor(infc, classType, lifetimeForClass));
                    result.Add(new AutoRegisteredResult(classType, infc, lifetimeForClass));
                }
            }

            return result;
        }
    }
}