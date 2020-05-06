namespace NetCore.AutoRegisterDi.Attributes
{
    using System;

    /// <summary>
    /// Attribute for marking classes which no need to register in container 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DoNotAutoRegisterAttribute : Attribute
    {
    }
}