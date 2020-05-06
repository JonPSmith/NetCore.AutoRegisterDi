namespace NetCore.AutoRegisterDi.Attributes
{
    using System;

    /// <summary>
    /// Attribute for marking classes which need to register with Scope life time
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterAsScopedAttribute : Attribute
    {
    }
}