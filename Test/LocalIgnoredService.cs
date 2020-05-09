namespace Test
{
    using NetCore.AutoRegisterDi.Attributes;

    [DoNotAutoRegister]
    public class LocalIgnoredService : ILocalService
    {
        public bool IsPositive(int i)
        {
            return i > 1;
        }
    }
}