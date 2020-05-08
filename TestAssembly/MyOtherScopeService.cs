namespace TestAssembly
{
    using NetCore.AutoRegisterDi.Attributes;

    [RegisterAsScoped]
    public class MyOtherScopeService : IMyOtherService
    {
        public string Result { get; }
    }
}