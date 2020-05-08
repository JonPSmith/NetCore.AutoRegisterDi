namespace Test
{
    using Microsoft.Extensions.DependencyInjection;
    using NetCore.AutoRegisterDi;
    using NetCore.AutoRegisterDi.Attributes;
    using Xunit;
    using Xunit.Extensions.AssertExtensions;

    public class TestTypeExtensions
    {
        [DoNotAutoRegister]
        private class Ignore
        {
        }

        [RegisterAsScoped]
        private class Scope
        {
        }

        [RegisterAsTransient]
        private class Transient
        {
        }

        [RegisterAsSingleton]
        private class Singleton
        {
        }

        [RegisterAsSingleton]
        [RegisterAsTransient]
        private class Multiple
        {
        }

        [Fact]
        public void IsIgnore()
        {
            typeof(Ignore).IsIgnoredType()
                .ShouldBeTrue();
            typeof(Scope).IsIgnoredType()
                .ShouldBeFalse();
        }

        [Fact]
        public void IsMultipleLifetime()
        {
            typeof(Multiple).IsMultipleLifetime()
                .ShouldBeTrue();
            typeof(Singleton).IsMultipleLifetime()
                .ShouldBeFalse();
        }

        [Fact]
        public void GetScopeLifetime()
        {
            typeof(Scope).GetTypeLiteTime().ShouldEqual(ServiceLifetime.Scoped);
        }

        [Fact]
        public void GetTransientLifetime()
        {
            typeof(Transient).GetTypeLiteTime().ShouldEqual(ServiceLifetime.Transient);
        }

        [Fact]
        public void GetSingletonLifetime()
        {
            typeof(Singleton).GetTypeLiteTime().ShouldEqual(ServiceLifetime.Singleton);
        }
    }
}