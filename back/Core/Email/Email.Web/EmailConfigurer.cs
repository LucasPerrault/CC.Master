using Email.Domain;
using Email.Infra;
using Email.Infra.Configuration;
using Lucca.Emails.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Email.Web
{
    public class EmailConfigurer
    {
        public static void ConfigureServices(IServiceCollection service, EmailConfiguration configuration)
        {
            service.AddSingleton(configuration);
            service.AddScoped<IEmailService, EmailService>();
            service.AddLuccaEmails<EmailUriProvider>(configuration);
        }
    }
}
