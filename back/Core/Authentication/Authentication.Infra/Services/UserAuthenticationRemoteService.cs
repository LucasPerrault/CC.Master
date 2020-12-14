using Authentication.Domain;
using Authentication.Infra.DTOs;
using Newtonsoft.Json;
using Partenaires.Infra.Configuration;
using Remote.Infra.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Authentication.Infra.Services
{
    public class UserAuthenticationRemoteService : RestApiV3HostRemoteService<PartenairesAuthServiceConfiguration>
    {
        private const string _authScheme = "Lucca";
        private const string _authType = "user";
        protected override string RemoteApiDescription => "Partenaires";

        public UserAuthenticationRemoteService(HttpClient httpClient, JsonSerializer jsonSerializer)
            : base(httpClient, jsonSerializer)
        { }

        public async Task<Principal> GetUserPrincipalAsync(Guid token)
        {
            var partenairesAuthConfig = new PartenairesAuthServiceConfiguration();
            partenairesAuthConfig.Authenticate(_httpClient, _authScheme, _authType, token);

            var queryParams = new Dictionary<string, string> { { "fields", LuccaUser.ApiFields } };

            try
            {
                var luccaUser = await GetObjectResponseAsync<LuccaUser>(queryParams);

                var user = luccaUser.Data.ToUser();
                return new Principal
                {
                    Token = token,
                    UserId = user.Id,
                    User = user
                };
            }
            catch
            {
                return null;
            }

        }
    }
}
