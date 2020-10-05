// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;
using Test.DifferentServices;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test
{
    public class TestAutoRegisterResult
    {

        [Fact]
        public void TestAsPublicImplementedInterfacesMyService()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            var results = service.RegisterAssemblyPublicNonGenericClasses()
                .AsPublicImplementedInterfaces();

            //VERIFY
            results.Count.ShouldEqual(7);
            var resultsAsString = results.Select(x => x.ToString()).ToList();
            resultsAsString[0].ShouldEqual("The interface IDisposable is ignored");
            resultsAsString[1].ShouldEqual("The interface ISerializable is ignored");

            resultsAsString.ShouldContain("LocalService : ILocalService (Transient)");
            resultsAsString.ShouldContain("LocalService : IAnotherInterface (Transient)");
            resultsAsString.ShouldContain("LocalSingletonService : ILocalService (Singleton)");
            resultsAsString.ShouldContain("LocalTransientService : ILocalService (Transient)");
            resultsAsString.ShouldContain("LocalScopeService : ILocalService (Scoped)");
        }

        [Fact]
        public void TestAsPublicImplementedInterfacesWithIgnore()
        {
            //SETUP
            var service = new ServiceCollection();

            //ATTEMPT
            var results = service.RegisterAssemblyPublicNonGenericClasses()
                .IgnoreThisInterface<IAnotherInterface>()
                .AsPublicImplementedInterfaces();

            //VERIFY
            results.Count.ShouldEqual(7);
            var resultsAsString = results.Select(x => x.ToString()).ToList();
            resultsAsString[0].ShouldEqual("The interface IDisposable is ignored");
            resultsAsString[1].ShouldEqual("The interface ISerializable is ignored");
            resultsAsString[2].ShouldEqual("The interface IAnotherInterface is ignored");

            resultsAsString.ShouldContain("LocalService : ILocalService (Transient)");
            resultsAsString.ShouldContain("LocalSingletonService : ILocalService (Singleton)");
            resultsAsString.ShouldContain("LocalTransientService : ILocalService (Transient)");
            resultsAsString.ShouldContain("LocalScopeService : ILocalService (Scoped)");
        }

        
    }
}