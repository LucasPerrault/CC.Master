using Microsoft.Extensions.DependencyInjection;

namespace Lock.Web
{
    public static class LockConfigurer
    {
        public static void ConfigureLock(IServiceCollection service, string databaseConnection)
        {
            service.AddSingleton<ILockService>(new LockService(databaseConnection));
        }
    }
}
