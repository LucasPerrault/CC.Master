using Remote.Infra.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Users.Domain;

namespace Users.Infra
{
    public class UsersService : RestApiV3HostRemoteService, IUsersService
    {
        protected override string RemoteApiDescription => "Partenaires users";
        public UsersService(HttpClient httpClient) : base(httpClient)
        { }

        public async Task<User> GetByTokenAsync(Guid token)
        {
            ApplyLateHttpClientAuthentication("Lucca", a => a.AuthenticateAsUser(token));

            var queryParams = new Dictionary<string, string> { { "fields", LuccaUser.ApiFields } };
            var luccaUser = await GetObjectResponseAsync<LuccaUser>(queryParams);
            return luccaUser.Data.ToUser();
        }

    }
}
