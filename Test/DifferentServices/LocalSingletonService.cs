using NetCore.AutoRegisterDi;

namespace Test.DifferentServices
{
    [RegisterAsSingleton]
    public class LocalSingletonService : ILocalService
    {
        public bool IsPositive(int i)
        {
            return i > 1;
        }
    }
}