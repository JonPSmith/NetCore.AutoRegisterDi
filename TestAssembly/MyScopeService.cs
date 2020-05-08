namespace TestAssembly
{
    using NetCore.AutoRegisterDi.Attributes;

    [RegisterAsScoped]
    public class MyScopeService : IMyService
    {
        public string IntToString(int num)
        {
            return num.ToString();
        }
    }
}