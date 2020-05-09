// Copyright (c) 2018 Inventory Innovations, Inc. - build by Jon P Smith (GitHub JonPSmith)
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Extensions.AssertExtensions;
    using Microsoft.Extensions.DependencyInjection;
    using NetCore.AutoRegisterDi;
    using TestAssembly;
    using TestBadAssembly;
    using Xunit;
    using Xunit.Extensions.AssertExtensions;

    public class TestAutoRegisterDiDifferentAssembly
    {
        [Fact]
        public void TestRegisterAssemblyPublicNonGenericClasses()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            var autoRegData = service.RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(MyService)));

            //VERIFY
            //does not contain nested class
            autoRegData.TypesToConsider.ShouldEqual(
                new[]
                {
                    typeof(ClassWithNestedService),
                    typeof(MyOtherScopeService), typeof(MyOtherService),
                    typeof(MyScopeService), typeof(MyService),
                    typeof(UseService)
                });
        }

        [Fact]
        public void TestAsPublicImplementedInterfacesMyService()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            service.RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(MyService)))
                .AsPublicImplementedInterfaces();

            //VERIFY
            service.Count.ShouldEqual(4);
            service.Contains(new ServiceDescriptor(typeof(IMyService), typeof(MyService), ServiceLifetime.Transient),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(
                new ServiceDescriptor(typeof(IMyOtherService), typeof(MyOtherService), ServiceLifetime.Transient),
                new CheckDescriptor()).ShouldBeTrue();
        }

        [Fact]
        public void TestAsPublicImplementedInterfacesMyServiceSetLifetime()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            service.RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(MyService)))
                .AsPublicImplementedInterfaces();

            //VERIFY
            service.Count.ShouldEqual(4);
            service.Contains(new ServiceDescriptor(typeof(IMyService), typeof(MyScopeService), ServiceLifetime.Scoped),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(
                new ServiceDescriptor(typeof(IMyOtherService), typeof(MyOtherScopeService), ServiceLifetime.Scoped),
                new CheckDescriptor()).ShouldBeTrue();
        }

        [Fact]
        public void TestWhere()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            service.RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(MyService)))
                .Where(x => x.Name == nameof(MyService))
                .AsPublicImplementedInterfaces();

            //VERIFY
            service.Count.ShouldEqual(1);
            service.Contains(new ServiceDescriptor(typeof(IMyService), typeof(MyService), ServiceLifetime.Transient),
                new CheckDescriptor()).ShouldBeTrue();
        }

        [Fact]
        public void MultipleLifetimeRegistrationShouldThrowException()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            Action action = () => service
                .RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(BadService)))
                .Where(x => x.Name.Contains("Service"))
                .AsPublicImplementedInterfaces();

            //VERIFY
            action.ShouldThrow<ArgumentException>();
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