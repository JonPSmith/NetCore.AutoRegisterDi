namespace TestBadAssembly
{
    using System;
    using NetCore.AutoRegisterDi.Attributes;

    [RegisterAsSingleton]
    [RegisterAsScoped]
    public class BadService : IService
    {
        public void Method()
        {
            throw new NotImplementedException();
        }
    }
}