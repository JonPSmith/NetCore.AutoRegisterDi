namespace NetCore.AutoRegisterDi.Attributes
{
    using System;

    /// <summary>
    /// Attribute for marking classes which need to register with Scope lifetime
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterAsScopedAttribute : Attribute
    {
    }
}