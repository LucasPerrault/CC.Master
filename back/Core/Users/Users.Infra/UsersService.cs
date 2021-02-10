using Newtonsoft.Json;
using Remote.Infra.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Users.Domain;
using Users.Infra.Storage.Stores;

namespace Users.Infra
{
    public class UsersService : RestApiV3HostRemoteService, IUsersService
    {
        private readonly UsersStore _store;
        protected override string RemoteApiDescription => "Partenaires users";
        public UsersService(HttpClient httpClient, JsonSerializer jsonSerializer, UsersStore store)
            : base(httpClient, jsonSerializer)
        {
            _store = store;
        }

        public async Task<User> GetByTokenAsync(Guid token)
        {
            ApplyLateHttpClientAuthentication("Lucca", a => a.AuthenticateAsUser(token));

            var queryParams = new Dictionary<string, string> { { "fields", LuccaUser.ApiFields } };

            try
            {
                var luccaUser = await GetObjectResponseAsync<LuccaUser>(queryParams);
                var user = luccaUser.Data.ToUser();
                if (user == null)
                {
                    return null;
                }

                if (!await _store.ExistsByIdAsync(user.Id))
                {
                    await _store.CreateAsync(user);
                }

                return user;
            }
            catch
            {
                return null;
            }
        }

    }
}
