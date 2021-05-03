using Email.Infra.Configuration;
using Lucca.Emails.Client;
using System;

namespace Email.Web
{
    public class EmailUriProvider : ILuccaEmailsUriProvider
    {
        private readonly Uri _emailAppUri;

        public EmailUriProvider(EmailConfiguration configuration)
        {
            _emailAppUri = new Uri(configuration.ServerUri, $"{configuration.EmailAppEndpointPath}/");
        }

        public Uri GetUri() => _emailAppUri;
    }
}
