using Remote.Infra.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Users.Domain;

namespace Users.Infra
{
    public class UsersService : IUsersService
    {
        private readonly RestApiV3HttpClientHelper _httpClientHelper;

        public UsersService(HttpClient httpClient)
        {
            _httpClientHelper = new RestApiV3HttpClientHelper(httpClient, "Partenaires users");
        }

        public async Task<User> GetByTokenAsync(Guid token)
        {
            _httpClientHelper.ApplyLateHttpClientAuthentication("Lucca", a => a.AuthenticateAsUser(token));

            var queryParams = new Dictionary<string, string> { { "fields", LuccaUser.ApiFields } };
            var luccaUser = await _httpClientHelper.GetObjectResponseAsync<LuccaUser>(queryParams);
            return luccaUser.Data.ToUser();
        }

    }
}
