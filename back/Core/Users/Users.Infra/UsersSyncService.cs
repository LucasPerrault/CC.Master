using Distributors.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Remote.Infra.Services;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Users.Domain;
using Users.Domain.Filtering;
using Users.Infra.Storage.Stores;

namespace Users.Infra
{
    public class UsersSyncService : IUsersSyncService
    {
        public static readonly DateTime EarliestContractEnd = new DateTime(1900, 1, 1);

        private readonly UsersStore _store;
        private readonly ILogger<UsersSyncService> _logger;
        private readonly IDistributorsStore _distributorsStore;
        private readonly RestApiV3HttpClientHelper _httpClientHelper;
        protected override string RemoteApiDescription => "Partenaires users";

        public UsersSyncService
        (
            HttpClient httpClient,
            IDistributorsStore distributorsStore,
            UsersStore store,
            ILogger<UsersSyncService> logger
        )
        {
            _distributorsStore = distributorsStore;
            _httpClientHelper = new RestApiV3HttpClientHelper(httpClient, "Partenaires users");
            _store = store;
            _logger = logger;
        }

        public async Task SyncAsync()
        {
            var allUsersFromRemote = await GetUsersFromRemoteAsync();

            var localUsers = await _store.GetAllAsync(new UsersFilter(), AccessRight.All);
            var localUsersDict = localUsers.ToDictionary(u => u.Id, u => u);

            var distributorsPerCode = ( await _distributorsStore.GetAllAsync() )
                .ToDictionary(d => d.Code, d => d);

            foreach (var remoteUser in allUsersFromRemote)
            {
                var departmentCode = remoteUser.Department.Code;
                if (!distributorsPerCode.TryGetValue(departmentCode, out var distributor))
                {
                    _logger.LogError($"User {remoteUser.Id} has unknown department code {departmentCode}");
                    continue;
                }

                if (localUsersDict.TryGetValue(remoteUser.Id, out var user))
                {
                    user.FirstName = remoteUser.FirstName;
                    user.LastName = remoteUser.LastName;
                    user.DepartmentId = remoteUser.DepartmentId;
                    user.DistributorId = distributor.Id;
                    user.IsActive = remoteUser.IsActive;
                }
                else
                {
                    var simpleUser = new SimpleUser
                    {
                        Id = remoteUser.Id,
                        DepartmentId = remoteUser.DepartmentId,
                        DistributorId = distributor.Id,
                        FirstName = remoteUser.FirstName,
                        LastName = remoteUser.LastName,
                        IsActive = remoteUser.IsActive
                    };

                    await _store.CreateAsync(simpleUser);
                }
            }

            await _store.SaveChangesAsync();
        }

        private async Task<ICollection<LuccaUser>> GetUsersFromRemoteAsync()
        {
            var queryParams = new Dictionary<string, string>
            {
                { "fields", LuccaUser.ApiFields },
                {"dtContractEnd", $"since,{EarliestContractEnd :yyyy-MM-dd},null"}
            };
            var response = await _httpClientHelper.GetObjectCollectionResponseAsync<LuccaUser>(queryParams);
            return response.Data.Items;
        }
    }
}
