// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;
using Test.DifferentServices;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test
{
    public class TestAutoRegisterDiCallingAssembly
    {
        [Fact]
        public void TestRegisterAssemblyPublicNonGenericClasses()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            var autoRegData = service.RegisterAssemblyPublicNonGenericClasses();

            //VERIFY
            //does not contain nested class
            autoRegData.TypesToConsider.ShouldEqual(
                new[]
                {
                    typeof(TestAutoRegisterDiCallingAssembly),
                    typeof(TestAutoRegisterDiDifferentAssembly),
                    typeof(TestAutoRegisterResult),
                    typeof(TestTypeExtensions),
                    typeof(ClassWithJustIDisposable),
                    typeof(ClassWithJustISerializable),
                    typeof(LocalScopeService), typeof(LocalService),
                    typeof(LocalSingletonService), typeof(LocalTransientService),
                    typeof(MyException)
                });
        }

        [Fact]
        public void TestAsPublicImplementedInterfacesMyService()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            service.RegisterAssemblyPublicNonGenericClasses()
                .AsPublicImplementedInterfaces();

            //VERIFY
            service.Count.ShouldEqual(5);
            service.Contains(new ServiceDescriptor(typeof(ILocalService), typeof(LocalService), ServiceLifetime.Transient),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(IAnotherInterface), typeof(LocalService), ServiceLifetime.Transient),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(ILocalService), typeof(LocalSingletonService), ServiceLifetime.Singleton),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(ILocalService), typeof(LocalTransientService), ServiceLifetime.Transient),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(ILocalService), typeof(LocalScopeService), ServiceLifetime.Scoped),
                new CheckDescriptor()).ShouldBeTrue();
        }

        [Fact]
        public void TestAsPublicImplementedInterfacesMyServiceSetLifetime()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            service.RegisterAssemblyPublicNonGenericClasses()
                .AsPublicImplementedInterfaces(ServiceLifetime.Singleton);

            //VERIFY
            service.Count.ShouldEqual(5);
            service.Contains(new ServiceDescriptor(typeof(ILocalService), typeof(LocalService), ServiceLifetime.Singleton),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(IAnotherInterface), typeof(LocalService), ServiceLifetime.Singleton),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(ILocalService), typeof(LocalSingletonService), ServiceLifetime.Singleton),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(ILocalService), typeof(LocalTransientService), ServiceLifetime.Transient),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(ILocalService), typeof(LocalScopeService), ServiceLifetime.Scoped),
                new CheckDescriptor()).ShouldBeTrue();
        }

        [Fact]
        public void TestSingleWhere()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            service.RegisterAssemblyPublicNonGenericClasses()
                .Where(x => x.Name == nameof(LocalService))
                .AsPublicImplementedInterfaces();

            //VERIFY
            service.Count.ShouldEqual(2);
            service.Contains(new ServiceDescriptor(typeof(ILocalService), typeof(LocalService), ServiceLifetime.Transient),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(IAnotherInterface), typeof(LocalService), ServiceLifetime.Transient),
                new CheckDescriptor()).ShouldBeTrue();
        }

        [Fact]
        public void TestMultipleWhere()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            service.RegisterAssemblyPublicNonGenericClasses()
                .Where(x => x.Name != nameof(LocalService))
                .Where(x => x.Name != nameof(LocalScopeService))
                .AsPublicImplementedInterfaces();

            //VERIFY
            service.Count.ShouldEqual(2);
            service.Contains(new ServiceDescriptor(typeof(ILocalService), typeof(LocalSingletonService), ServiceLifetime.Singleton),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(ILocalService), typeof(LocalTransientService), ServiceLifetime.Transient),
                new CheckDescriptor()).ShouldBeTrue();
        }

        [Fact]
        public void TestSingleIgnoreThisInterface()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            service.RegisterAssemblyPublicNonGenericClasses()
                .IgnoreThisInterface<IAnotherInterface>()
                .AsPublicImplementedInterfaces();

            //VERIFY
            service.Count.ShouldEqual(4);
            service.Contains(new ServiceDescriptor(typeof(ILocalService), typeof(LocalService), ServiceLifetime.Transient),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(ILocalService), typeof(LocalSingletonService), ServiceLifetime.Singleton),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(ILocalService), typeof(LocalTransientService), ServiceLifetime.Transient),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(ILocalService), typeof(LocalScopeService), ServiceLifetime.Scoped),
                new CheckDescriptor()).ShouldBeTrue();
        }

        //-------------------------------------------------------------------------
        //private methods

        private class CheckDescriptor : IEqualityComparer<ServiceDescriptor>
        {
            public bool Equals(ServiceDescriptor x, ServiceDescriptor y)
            {
                return x.ServiceType == y.ServiceType
                       && x.ImplementationType == y.ImplementationType
                       && x.Lifetime == y.Lifetime;
            }

            public int GetHashCode(ServiceDescriptor obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}