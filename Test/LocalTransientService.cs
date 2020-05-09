namespace Test
{
    using NetCore.AutoRegisterDi.Attributes;

    [RegisterAsTransient]
    public class LocalTransientService : ILocalService
    {
        public bool IsPositive(int i)
        {
            return i > 1;
        }
    }
}