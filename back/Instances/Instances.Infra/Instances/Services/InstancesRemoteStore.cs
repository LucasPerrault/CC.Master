using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Remote.Infra.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Instances.Infra.Instances.Services
{
    public class InstancesRemoteStore : IInstancesStore
    {

        private readonly RestApiV3HttpClientHelper _httpClientHelper;

        public InstancesRemoteStore(HttpClient httpClient)
        {
            _httpClientHelper = new RestApiV3HttpClientHelper(httpClient, "Legacy cloudcontrol instances api");
        }

        private class CreateForDemoDto
        {
            public string Password { get; set; }
            public string Cluster { get; set; }
        }

        public async Task<Instance> CreateForDemoAsync(string password, string cluster)
        {
            var dto = new CreateForDemoDto { Password = password, Cluster = cluster};
            var response = await _httpClientHelper.PostObjectResponseAsync<CreateForDemoDto, Instance>
            (
                "createForDemo",
                dto,
                new Dictionary<string, string>()
            );

            return response.Data;
        }

        public class DeleteForDemoDto
        {
            public int Id { get; set; }
        }

        public async Task DeleteForDemoAsync(Instance demoInstance)
        {
            await _httpClientHelper.PostObjectResponseAsync<DeleteForDemoDto, Instance>
            (
                "deleteForDemo",
                new DeleteForDemoDto { Id =  demoInstance.Id },
                new Dictionary<string, string>()
            );
        }

        public async Task DeleteForDemoAsync(IEnumerable<Instance> demoInstances)
        {
            foreach (var demoInstance in demoInstances)
            {
                await DeleteForDemoAsync(demoInstance);
            }
        }
    }
}
