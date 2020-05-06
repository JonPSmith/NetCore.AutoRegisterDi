namespace NetCore.AutoRegisterDi.Attributes
{
    using System;

    /// <summary>
    /// Attribute for marking classes which need to register with Singleton life time
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterAsSingletonAttribute : Attribute
    {
    }
}