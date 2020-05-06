namespace NetCore.AutoRegisterDi.Attributes
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Attribute for marking classes which need to register with Transient life time
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterAsTransientAttribute : Attribute
    {
    }
}