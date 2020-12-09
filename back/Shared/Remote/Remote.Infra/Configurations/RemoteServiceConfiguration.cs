using Remote.Infra.Extensions;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Remote.Infra.Configurations
{
    public abstract class RemoteServiceConfiguration<T> where T : IHttpClientConfiguration
    {
        protected const string _userAgentKey = "User-Agent";
        private const string _cloudControlUserAgent = "CloudControl";

        protected static string AuthorizationHeaderParam(Guid token, string type) => $"{type}={token.ToString()}";
        protected static string GetUserAgent(string suffix) => $"{_cloudControlUserAgent} {suffix}";

        public abstract void Configure(HttpClient client, T httpClientConfiguration);
        public abstract void Authenticate(HttpClient client, string authorizationScheme, string authorizationType, Guid authToken);
    }

    public class RemoteServiceConfiguration : RemoteServiceConfiguration<HostHttpClientConfiguration>
    {
        private readonly string _userAgent;
        private readonly string _authScheme;
        private readonly string _authType;
        private readonly Guid? _authToken = null;

        public RemoteServiceConfiguration(string userAgent)
        {
            _userAgent = userAgent;
        }

        public RemoteServiceConfiguration(Guid authToken, string userAgent, string authScheme, string authType)
            : this(userAgent)
        {
            _authScheme = authScheme;
            _authType = authType;
            _authToken = authToken;
        }

        public override void Configure(HttpClient client, HostHttpClientConfiguration httpClientConfiguration)
        {
            client.SetSafeBaseAddress(httpClientConfiguration.Endpoint);
            client.DefaultRequestHeaders.Add(_userAgentKey, GetUserAgent(_userAgent));

            if (_authToken.HasValue && !string.IsNullOrEmpty(_authScheme))
            {
                Authenticate(client, _authScheme, _authType, _authToken.Value);
            }
        }

        public override void Authenticate(HttpClient client, string authScheme, string authType, Guid authToken)
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue
                (
                    authScheme,
                    AuthorizationHeaderParam(authToken, authType)
                );
        }
    }
}
