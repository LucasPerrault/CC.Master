using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Remote.Infra.Services;
using System.Collections.Generic;
using System.Linq;
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
        }

        private class CreateForTrainingDto
        {
            public int EnvironmentId { get; set; }
            public bool IsAnonymized { get; set; }
        }

        public async Task<Instance> CreateForTrainingAsync(int environmentId, bool isAnonymized)
        {
            var dto = new CreateForTrainingDto { EnvironmentId = environmentId, IsAnonymized = isAnonymized };
            var response = await _httpClientHelper.PostObjectResponseAsync<CreateForTrainingDto, Instance>
            (
                "createForTraining",
                dto,
                new Dictionary<string, string>()
            );

            return response.Data;
        }

        public async Task<Instance> CreateForDemoAsync(string password)
        {
            var dto = new CreateForDemoDto { Password = password };
            var response = await _httpClientHelper.PostObjectResponseAsync<CreateForDemoDto, Instance>
            (
                "createForDemo",
                dto,
                new Dictionary<string, string>()
            );

            return response.Data;
        }

        public class DeleteDto
        {
            public int Id { get; set; }
        }

        public async Task DeleteByIdAsync(int instanceId)
        {
            await _httpClientHelper.PostObjectResponseAsync<DeleteDto, Instance>
            (
                "deleteForCCMaster",
                new DeleteDto { Id =  instanceId },
                new Dictionary<string, string>()
            );
        }

        public async Task DeleteByIdsAsync(IEnumerable<int> instanceIds)
        {
            foreach (var instanceId in instanceIds)
            {
                await DeleteByIdAsync(instanceId);
            }
        }
    }
}
