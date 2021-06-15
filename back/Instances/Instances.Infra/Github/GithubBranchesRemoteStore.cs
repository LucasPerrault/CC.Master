using Instances.Domain.CodeSources;
using Newtonsoft.Json;
using Remote.Infra.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Instances.Infra.Github
{
    public class GithubBranchesRemoteStore : RestApiV3HostRemoteService, IGithubBranchesStore
    {
        protected override string RemoteApiDescription => "Legacy cloudcontrol github branches api";

        public GithubBranchesRemoteStore(HttpClient httpClient, JsonSerializer jsonSerializer)
            : base(httpClient, jsonSerializer)
        { }

        public async Task CreateForNewSourceCodeAsync(CodeSource codeSource)
        {
            var payload = new { Id = codeSource.Id };

            await PostObjectResponseAsync<object, object>
            (
                "createDefaultBranches",
                payload,
                new Dictionary<string, string>()
            );
        }
    }
}
