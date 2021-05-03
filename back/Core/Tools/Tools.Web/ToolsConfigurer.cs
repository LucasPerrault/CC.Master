using Microsoft.Extensions.DependencyInjection;

namespace Tools.Web
{
    public static class ToolsConfigurer
    {
        public static void ConfigureTools(IServiceCollection service)
        {
            service.AddSingleton<ITimeProvider, TimeProvider>();
        }
    }
}
