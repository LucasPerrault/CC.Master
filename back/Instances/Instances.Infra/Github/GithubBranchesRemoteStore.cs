using Instances.Domain.CodeSources;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Remote.Infra.Exceptions;
using Remote.Infra.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Instances.Infra.Github
{
    public class GithubBranchesRemoteStore : RestApiV3HostRemoteService, IGithubBranchesStore
    {
        private readonly ILogger<GithubBranchesRemoteStore> _logger;
        protected override string RemoteApiDescription => "Legacy cloudcontrol github branches api";

        public GithubBranchesRemoteStore(HttpClient httpClient, JsonSerializer jsonSerializer, ILogger<GithubBranchesRemoteStore> logger)
            : base(httpClient, jsonSerializer)
        {
            _logger = logger;
        }

        public async Task CreateForNewSourceCodeAsync(CodeSource codeSource)
        {
            var payload = new { Id = codeSource.Id };

            try
            {
                await PostObjectResponseAsync<object, object>
                (
                    "createDefaultBranches",
                    payload,
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
