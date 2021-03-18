using CloudControl.Web;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CloudControl.Web.Tests.Mocks
{
    public class TestHostBuilder<TAuthenticationHandler> where TAuthenticationHandler : AuthenticationHandler<TestAuthenticationOptions>
    {
        public static IWebHostBuilder GetInMemory()
        {
            return WebHost.CreateDefaultBuilder(null)
                .UseEnvironment("Development")
                .ConfigureServices(s =>
                {
                    s.AddSingleton<ServicesConfiguration, TestServicesConfiguration<TAuthenticationHandler>>();
                })
                .UseStartup<Startup>();
        }
    }
}
