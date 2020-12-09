using Authentication.Domain;
using Core.Proxy.Infra.Configuration;
using Newtonsoft.Json;
using Remote.Infra.Services;
using System;
using System.Net.Http;
using System.Security.Claims;

namespace Core.Proxy.Infra.Services
{
    public abstract class LegacyCcAuthenticatedRemoteService : RestApiV3HostRemoteService<LegacyCloudControlServiceConfiguration>
    {
        private const string _authScheme = "CloudControl";
        protected override string RemoteAppName => LegacyCloudControlServiceConfiguration.RemoteAppName;
        protected readonly ClaimsPrincipal _claimsPrincipal;

        protected LegacyCcAuthenticatedRemoteService(HttpClient httpClient, JsonSerializer jsonSerializer, ClaimsPrincipal claimsPrincipal)
            : base(httpClient, jsonSerializer)
        {
            _claimsPrincipal = claimsPrincipal ?? throw new ArgumentNullException(nameof(claimsPrincipal));

            Authenticate(_claimsPrincipal);
        }

        protected void Authenticate(ClaimsPrincipal claimsPrincipal)
        {
            var partenairesAuthConfig = new LegacyCloudControlServiceConfiguration();

            string authType;
            Guid token;

            switch (claimsPrincipal)
            {
                case CloudControlUserClaimsPrincipal u:
                    authType = "user";
                    token = u.Token;
                    break;
                case CloudControlApiKeyClaimsPrincipal ak:
                    authType = "application";
                    token = ak.Token;
                    break;
                default:
                    throw new ApplicationException("Can't authenticate to CloudControl service with unrecognized principal");
            }

            partenairesAuthConfig.Authenticate(_httpClient, _authScheme, authType, token);
        }
    }
}
