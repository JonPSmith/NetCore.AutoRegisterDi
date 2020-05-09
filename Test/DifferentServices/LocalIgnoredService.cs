using NetCore.AutoRegisterDi;

namespace Test.DifferentServices
{
    [DoNotAutoRegister]
    public class LocalIgnoredService : ILocalService
    {
        public bool IsPositive(int i)
        {
            return i > 1;
        }
    }
}