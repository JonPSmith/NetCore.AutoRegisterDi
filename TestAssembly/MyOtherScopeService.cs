using NetCore.AutoRegisterDi;

namespace TestAssembly
{
    [RegisterAsScoped]
    public class MyOtherScopeService : IMyOtherService
    {
        public string Result { get; }
    }
}