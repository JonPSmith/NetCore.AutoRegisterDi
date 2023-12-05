// Copyright (c) 2018 Inventory Innovations, Inc. - build by Jon P Smith (GitHub JonPSmith)
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Runtime.Serialization;
using Xunit.Abstractions;

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
        private readonly ITestOutputHelper _output;

        public TestAutoRegisterDiDifferentAssembly(ITestOutputHelper output)
        {
            _output = output;
        }

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
                    typeof(RecordNoInterface), typeof(RecordWithInterface), 
                    typeof(UseService)
                });
        }

        [Fact]
        public void TestAsPublicImplementedInterfaces_CheckServices()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            var logs= service.RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(MyService)))
                .AsPublicImplementedInterfaces();

            //VERIFY
            foreach (var log in logs)
            {
                _output.WriteLine(log.ToString());
            }
            service.Count.ShouldEqual(7);
            service.Contains(
                new ServiceDescriptor(typeof(IMyOtherService), typeof(MyOtherScopeService), ServiceLifetime.Scoped),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(
                new ServiceDescriptor(typeof(IMyOtherService), typeof(MyOtherService), ServiceLifetime.Transient),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(
                new ServiceDescriptor(typeof(IMyService), typeof(MyScopeService), ServiceLifetime.Scoped),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(
                new ServiceDescriptor(typeof(IMyService), typeof(MyService), ServiceLifetime.Transient),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(
                new ServiceDescriptor(typeof(IRecordWithInterface), typeof(RecordWithInterface), ServiceLifetime.Transient),
                new CheckDescriptor()).ShouldBeTrue();
            //records
            service.Contains(
                new ServiceDescriptor(typeof(IEquatable<RecordWithInterface>), typeof(RecordWithInterface), ServiceLifetime.Transient),
                new CheckDescriptor()).ShouldBeTrue();
            service.Contains(
                new ServiceDescriptor(typeof(IEquatable<RecordNoInterface>), typeof(RecordNoInterface), ServiceLifetime.Transient),
                new CheckDescriptor()).ShouldBeTrue();
        }

        [Fact]
        public void TestAsPublicImplementedInterfaces_IgnoreIMyOtherService()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            var logs = service.RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(MyService)))
                .IgnoreThisInterface<IMyOtherService>()
                .AsPublicImplementedInterfaces();

            //VERIFY
            foreach (var log in logs)
            {
                _output.WriteLine(log.ToString());
            }
            service.Count.ShouldEqual(5);
            logs[0].ToString().ShouldEqual("The interface IDisposable is ignored");
            logs[1].ToString().ShouldEqual("The interface ISerializable is ignored");
            logs[2].ToString().ShouldEqual("The interface IMyOtherService is ignored");
            logs[3].ToString().ShouldEqual("MyScopeService : IMyService (Scoped)");
            logs[4].ToString().ShouldEqual("MyService : IMyService (Transient)");
            logs[5].ToString().ShouldEqual("RecordNoInterface : IEquatable`1 (Transient)");
            logs[6].ToString().ShouldEqual("RecordWithInterface : IRecordWithInterface (Transient)");
            logs[7].ToString().ShouldEqual("RecordWithInterface : IEquatable`1 (Transient)");
        }

        [Fact]
        public void TestAsPublicImplementedInterfaces_IgnoreIEquatable()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            var logs = service.RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(MyService)))
                .IgnoreThisGenericInterface(typeof(IEquatable<>))
                .AsPublicImplementedInterfaces();

            //VERIFY
            foreach (var log in logs)
            {
                _output.WriteLine(log.ToString());
            }
            service.Count.ShouldEqual(5);
            logs[0].ToString().ShouldEqual("The interface IDisposable is ignored");
            logs[1].ToString().ShouldEqual("The interface ISerializable is ignored");
            logs[2].ToString().ShouldEqual("The interface IEquatable`1 is ignored");
            logs[3].ToString().ShouldEqual("MyOtherScopeService : IMyOtherService (Scoped)");
            logs[4].ToString().ShouldEqual("MyOtherService : IMyOtherService (Transient)");
            logs[5].ToString().ShouldEqual("MyScopeService : IMyService (Scoped)");
            logs[6].ToString().ShouldEqual("MyService : IMyService (Transient)");
            logs[7].ToString().ShouldEqual("RecordWithInterface : IRecordWithInterface (Transient)");
        }

        [Fact]
        public void TestWhere()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            var logs = service.RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(MyService)))
                .Where(x => x.Name == nameof(MyService))
                .AsPublicImplementedInterfaces();

            //VERIFY
            foreach (var log in logs)
            {
                _output.WriteLine(log.ToString());
            }
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