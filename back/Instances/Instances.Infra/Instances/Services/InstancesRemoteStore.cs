using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Newtonsoft.Json;
using Remote.Infra.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Instances.Infra.Instances.Services
{
    public class InstancesRemoteStore : RestApiV3HostRemoteService, IInstancesStore
    {

        protected override string RemoteApiDescription => "Legacy cloudcontrol instances api";

        public InstancesRemoteStore(HttpClient httpClient, JsonSerializer jsonSerializer) : base(httpClient, jsonSerializer)
        { }


        class CreateForDemoDto
        {
            public string Password { get; set; }
            public string Server { get; set; }
        }

        public async Task<Instance> CreateForDemoAsync(string password, string cluster)
        {
            var dto = new CreateForDemoDto { Password = password, Server = cluster};
            var response = await PostObjectResponseAsync<CreateForDemoDto, Instance>
            (
                "createForDemo",
                dto,
                new Dictionary<string, string>()
            );

            return response.Data;
        }
    }
}
