namespace NetCore.AutoRegisterDi.Attributes
{
    using System;

    /// <summary>
    /// Attribute for marking classes which need to register with Transient lifetime
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterAsTransientAttribute : Attribute
    {
    }
}