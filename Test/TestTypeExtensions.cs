namespace Test
{
    using Microsoft.Extensions.DependencyInjection;
    using NetCore.AutoRegisterDi;
    using Xunit;
    using Xunit.Extensions.AssertExtensions;

    public class TestTypeExtensions
    {
        private class NormalClass
        {
        }

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
        public void GetDefaultLifetime()
        {
            typeof(NormalClass).GetLifetimeForClass(ServiceLifetime.Transient).ShouldEqual(ServiceLifetime.Transient);
        }

        [Fact]
        public void GetScopeLifetime()
        {
            typeof(Scope).GetLifetimeForClass(ServiceLifetime.Transient).ShouldEqual(ServiceLifetime.Scoped);
        }

        [Fact]
        public void GetTransientLifetime()
        {
            typeof(Transient).GetLifetimeForClass(ServiceLifetime.Scoped).ShouldEqual(ServiceLifetime.Transient);
        }

        [Fact]
        public void GetSingletonLifetime()
        {
            typeof(Singleton).GetLifetimeForClass(ServiceLifetime.Transient).ShouldEqual(ServiceLifetime.Singleton);
        }
    }
}