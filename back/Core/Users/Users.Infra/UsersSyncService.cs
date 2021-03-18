using Newtonsoft.Json;
using Remote.Infra.Services;
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

        public Task SyncAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
