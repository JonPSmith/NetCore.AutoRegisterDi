using NetCore.AutoRegisterDi;

namespace Test.DifferentServices
{
    [RegisterAsScoped]
    public class LocalScopeService : ILocalService
    {
        public bool IsPositive(int i)
        {
            return i > 1;
        }
    }
}