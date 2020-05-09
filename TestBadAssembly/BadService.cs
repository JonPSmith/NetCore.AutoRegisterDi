using NetCore.AutoRegisterDi;

namespace TestBadAssembly
{
    using System;

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