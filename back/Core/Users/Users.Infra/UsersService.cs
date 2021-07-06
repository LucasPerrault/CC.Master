using Distributors.Domain;
using Newtonsoft.Json;
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
        private readonly IDistributorsStore _distributorsStore;
        protected override string RemoteApiDescription => "Partenaires users";
        public UsersService(HttpClient httpClient, JsonSerializer jsonSerializer, IDistributorsStore distributorsStore)
            : base(httpClient, jsonSerializer)
        {
            _distributorsStore = distributorsStore;
        }

        public async Task<User> GetByTokenAsync(Guid token)
        {
            ApplyLateHttpClientAuthentication("Lucca", a => a.AuthenticateAsUser(token));

            var queryParams = new Dictionary<string, string> { { "fields", LuccaUser.ApiFields } };
            var luccaUser = await GetObjectResponseAsync<LuccaUser>(queryParams);

            var userDepartmentCode = luccaUser.Data.Department.Code;
            var distributor = await _distributorsStore.GetByCodeAsync(luccaUser.Data.Department.Code);
            if (distributor is null)
            {
                throw new ApplicationException($"Unknown user department code {userDepartmentCode}");
            }

            return luccaUser.Data.ToUser(distributor);
        }
    }
}
