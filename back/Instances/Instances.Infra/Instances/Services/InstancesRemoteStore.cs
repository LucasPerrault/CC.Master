﻿using Instances.Domain.Instances;
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

        public async Task DeleteAsync(Instance instance)
        {
            await _httpClientHelper.PostObjectResponseAsync<DeleteDto, Instance>
            (
                "deleteForCCMaster",
                new DeleteDto { Id =  instance.Id },
                new Dictionary<string, string>()
            );
        }

        public async Task DeleteAsync(IEnumerable<Instance> instances)
        {
            foreach (var instance in instances)
            {
                await DeleteAsync(instance);
            }
        }
    }
}
