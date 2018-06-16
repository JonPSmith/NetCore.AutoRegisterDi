// Copyright (c) 2018 Inventory Innovations, Inc. - build by Jon P Smith (GitHub JonPSmith)
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test
{
    public class TestAutoRegisterDi
    {
        public interface INestedMyService {
            int MyInt { get; }
        }

        public class NestedMyService
        {
            public int MyInt { get; set; }
        }

        [Fact]
        public void TestRegisterAssemblyPublicNonGenericClasses()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMP
            var autoRegData = service.RegisterAssemblyPublicNonGenericClasses(Assembly.GetExecutingAssembly());

            //VERIFY
            //does not cotain nested class
            autoRegData.TypesToConsider.ShouldEqual( new []{typeof(MyOtherService), typeof(MyService), typeof(TestAutoRegisterDi) });
        }

        [Fact]
        public void TestAsPublicImplementedInterfacesMyService()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMP
            service.RegisterAssemblyPublicNonGenericClasses(Assembly.GetExecutingAssembly())
                .AsPublicImplementedInterfaces();

            //VERIFY
            service.Count.ShouldEqual(2);
            service.Contains(new ServiceDescriptor(typeof(IMyService), typeof(MyService), ServiceLifetime.Transient), new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(IMyOtherService), typeof(MyOtherService), ServiceLifetime.Transient), new CheckDescriptor()).ShouldBeTrue();
        }

        [Fact]
        public void TestAsPublicImplementedInterfacesMyServiceSetLifetime()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMP
            service.RegisterAssemblyPublicNonGenericClasses(Assembly.GetExecutingAssembly())
                .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

            //VERIFY
            service.Count.ShouldEqual(2);
            service.Contains(new ServiceDescriptor(typeof(IMyService), typeof(MyService), ServiceLifetime.Scoped), new CheckDescriptor()).ShouldBeTrue();
            service.Contains(new ServiceDescriptor(typeof(IMyOtherService), typeof(MyOtherService), ServiceLifetime.Scoped), new CheckDescriptor()).ShouldBeTrue();
        }

        [Fact]
        public void TestWhere()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMP
            var autoRegData = service.RegisterAssemblyPublicNonGenericClasses(Assembly.GetExecutingAssembly())
                .Where(x => x.Name == nameof(MyService))
                .AsPublicImplementedInterfaces();

            //VERIFY
            service.Count.ShouldEqual(1);
            service.Contains(new ServiceDescriptor(typeof(IMyService), typeof(MyService), ServiceLifetime.Transient), new CheckDescriptor()).ShouldBeTrue();
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
