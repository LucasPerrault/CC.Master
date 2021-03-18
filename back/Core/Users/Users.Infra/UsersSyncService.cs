﻿using Newtonsoft.Json;
using Remote.Infra.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Users.Domain;
using Users.Infra.Storage.Stores;

namespace Users.Infra
{
    public class UsersSyncService : RestApiV3HostRemoteService, IUsersSyncService
    {
        private readonly UsersStore _store;
        protected override string RemoteApiDescription => "Partenaires users";

        public UsersSyncService(HttpClient httpClient, JsonSerializer jsonSerializer, UsersStore store)
            : base(httpClient, jsonSerializer)
        {
            _store = store;
        }

        public async Task SyncAsync()
        {
            var allUsersFromRemote = await GetUsersFromRemoteAsync();

            var localUsers = await _store.GetAllAsync();
            var lucasUsersDict = localUsers.ToDictionary(u => u.Id, u => u);

            foreach (var remoteUser in allUsersFromRemote)
            {
                if (lucasUsersDict.TryGetValue(remoteUser.Id, out var user))
                {

                    user.FirstName = remoteUser.FirstName;
                    user.LastName = remoteUser.LastName;
                    user.DepartmentId = remoteUser.DepartmentId;
                }
                else
                {
                    await _store.CreateAsync(remoteUser.ToUser());
                }
            }

            await _store.SaveChangesAsync();
        }

        private async Task<ICollection<LuccaUser>> GetUsersFromRemoteAsync()
        {
            var queryParams = new Dictionary<string, string>
            {
                { "fields", LuccaUser.ApiFields }
            };
            var response = await GetObjectCollectionResponseAsync<LuccaUser>(queryParams);
            return response.Data.Items;
        }
    }
}
