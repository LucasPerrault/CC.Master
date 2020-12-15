using Authentication.Domain;
using Authentication.Infra.DTOs;
using Newtonsoft.Json;
using Remote.Infra.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Authentication.Infra.Services
{
    public class UserAuthenticationRemoteService : RestApiV3HostRemoteService
    {
        protected override string RemoteApiDescription => "Partenaires users";

        public UserAuthenticationRemoteService(HttpClient httpClient, JsonSerializer jsonSerializer)
            : base(httpClient, jsonSerializer)
        { }

        // will be called with token of current principal
        public async Task<Principal> GetUserPrincipalAsync(Guid token)
        {

            ApplyLateHttpClientAuthentication("Lucca", a => a.AuthenticateAsUser(token));

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
