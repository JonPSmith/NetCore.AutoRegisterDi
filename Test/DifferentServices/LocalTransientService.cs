using NetCore.AutoRegisterDi;

namespace Test.DifferentServices
{
    [RegisterAsTransient]
    public class LocalTransientService : ILocalService
    {
        public bool IsPositive(int i)
        {
            return i > 1;
        }
    }
}