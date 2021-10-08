using Instances.Domain.Github.Models;
using Instances.Domain.Preview;
using Instances.Domain.Preview.Models;
using Microsoft.Extensions.Logging;
using Remote.Infra.Exceptions;
using Remote.Infra.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Instances.Infra.Storage.Stores
{
    public class PreviewConfigurationsStore : IPreviewConfigurationsStore
    {
        private readonly ILogger<PreviewConfigurationsStore> _logger;
        private readonly RestApiV3HttpClientHelper _httpClientHelper;

        public PreviewConfigurationsStore(HttpClient httpClient, ILogger<PreviewConfigurationsStore> logger)
        {
            _logger = logger;
            _httpClientHelper = new RestApiV3HttpClientHelper(httpClient, "Legacy cloudcontrol preview configuration api");
        }

        public async Task CreateAsync(IEnumerable<PreviewConfiguration> previewConfigurations)
        {
            try
            {
                await _httpClientHelper.PostObjectResponseAsync<object>
                (
                    "createPreviews",
                    previewConfigurations.ToList(),
                    new Dictionary<string, string>()
                );
            }
            catch(RemoteServiceException e)
            {
                _logger.LogError(e, "Creation of preview configuration failed");
                throw;
            }
        }

        public async Task DeleteByBranchAsync(GithubBranch branch)
        {
            try
            {
                await _httpClientHelper.PostObjectResponseAsync<object>
                (
                    "deleteByBranch",
                    new { Id = branch.Id },
                    new Dictionary<string, string>()
                );
            }
            catch(RemoteServiceException e)
            {
                _logger.LogError(e, "Remove creation of preview configuration failed");
                throw;
            }
        }
    }
}
