namespace Test
{
    using NetCore.AutoRegisterDi.Attributes;

    [RegisterAsScoped]
    public class LocalScopeService : ILocalService
    {
        public bool IsPositive(int i)
        {
            return i > 1;
        }
    }
}