namespace Test.Extensions.AssertExtensions
{
    using System;
    using Xunit;

    /// <summary>
    /// Extensions which provide assertions to classes derived from <see cref="Action"/>.
    /// </summary>
    public static class ActionAssertionsExtensions
    {
        /// <summary>
        /// Verifies that the action throws exception.
        /// </summary>
        /// <param name="action">The action to be tested</param>
        /// <exception cref="ThrowsException">Thrown if the action is throw exception</exception>
        public static void ShouldThrow<T>(this Action action)
            where T : Exception
        {
            Assert.Throws<T>(action);
        }
    }
}