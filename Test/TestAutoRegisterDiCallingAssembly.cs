// Copyright (c) 2018 Inventory Innovations, Inc. - build by Jon P Smith (GitHub JonPSmith)
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;
using TestAssembly;
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
                new []{typeof(LocalService), typeof(TestAutoRegisterDiCallingAssembly), typeof(TestAutoRegisterDiDifferentAssembly)});
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
            service.Count.ShouldEqual(1);
            service.Contains(new ServiceDescriptor(typeof(ILocalService), typeof(LocalService), ServiceLifetime.Transient), new CheckDescriptor()).ShouldBeTrue();
        }

        [Fact]
        public void TestAsPublicImplementedInterfacesMyServiceSetLifetime()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            service.RegisterAssemblyPublicNonGenericClasses()
                .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

            //VERIFY
            service.Count.ShouldEqual(1);
            service.Contains(new ServiceDescriptor(typeof(ILocalService), typeof(LocalService), ServiceLifetime.Scoped), new CheckDescriptor()).ShouldBeTrue();
        }

        [Fact]
        public void TestWhere()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            service.RegisterAssemblyPublicNonGenericClasses()
                .Where(x => x.Name == nameof(MyService))
                .AsPublicImplementedInterfaces();

            //VERIFY
            service.Count.ShouldEqual(0);
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
