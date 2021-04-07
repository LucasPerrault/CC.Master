using Microsoft.Extensions.DependencyInjection;
using Remote.Infra.Services;

namespace Remote.Infra
{
    public static class RemoteConfigurer
    {
        public static void ConfigureRemote(IServiceCollection services)
        {
            services.AddSingleton<IHttpClientOAuthAuthenticator, HttpClientOAuthAuthenticator>();
            services.AddSingleton<HttpResponseMessageParser>();
        }
    }
}
