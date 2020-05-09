namespace Test
{
    using NetCore.AutoRegisterDi.Attributes;

    [RegisterAsSingleton]
    public class LocalSingletonService : ILocalService
    {
        public bool IsPositive(int i)
        {
            return i > 1;
        }
    }
}