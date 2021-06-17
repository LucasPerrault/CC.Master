using Instances.Domain.CodeSources;
using Microsoft.Extensions.Logging;
using Remote.Infra.Exceptions;
using Remote.Infra.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Instances.Infra.Github
{
    public class GithubBranchesRemoteStore : IGithubBranchesStore
    {
        private readonly ILogger<GithubBranchesRemoteStore> _logger;
        private readonly RestApiV3HttpClientHelper _httpClientHelper;

        public GithubBranchesRemoteStore(HttpClient httpClient, ILogger<GithubBranchesRemoteStore> logger)
        {
            _httpClientHelper = new RestApiV3HttpClientHelper(httpClient, "Legacy cloudcontrol github branches api");
            _logger = logger;
        }

        public async Task CreateForNewSourceCodeAsync(CodeSource codeSource)
        {
            try
            {
                await _httpClientHelper.PostObjectResponseAsync<object, object>
                (
                    "createDefaultBranches",
                    new { Id = codeSource.Id },
                    new Dictionary<string, string>()
                );
            }
            catch (RemoteServiceException e)
            {
                _logger.LogError(e, "Remote creation of default code source branches failed");
                throw;
            }
        }
    }
}
