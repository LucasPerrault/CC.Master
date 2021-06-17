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
        private readonly RestApiV3HttpClientHelper _httpClientHelper;

        public UsersSyncService(HttpClient httpClient, UsersStore store)
        {
            _httpClientHelper = new RestApiV3HttpClientHelper(httpClient, "Partenaires users");
            _store = store;
        }

        public async Task SyncAsync()
        {
            var allUsersFromRemote = await GetUsersFromRemoteAsync();

            var localUsers = await _store.GetAllAsync(new UsersFilter(), AccessRight.All);
            var localUsersDict = localUsers.ToDictionary(u => u.Id, u => u);

            foreach (var remoteUser in allUsersFromRemote)
            {
                if (localUsersDict.TryGetValue(remoteUser.Id, out var user))
                {
                    user.FirstName = remoteUser.FirstName;
                    user.LastName = remoteUser.LastName;
                    user.DepartmentId = remoteUser.DepartmentId;
                    user.IsActive = remoteUser.IsActive;
                }
                else
                {
                    var simpleUser = new SimpleUser
                    {
                        Id = remoteUser.Id,
                        DepartmentId = remoteUser.DepartmentId,
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
