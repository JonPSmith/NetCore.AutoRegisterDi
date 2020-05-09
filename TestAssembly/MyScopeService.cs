using NetCore.AutoRegisterDi;

namespace TestAssembly
{
    [RegisterAsScoped]
    public class MyScopeService : IMyService
    {
        public string IntToString(int num)
        {
            return num.ToString();
        }
    }
}