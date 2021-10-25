using Instances.Application.Webhooks.Harbor.Models;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Application.Webhooks.Harbor
{
    public class HarborWebhookService : IHarborWebhookService
    {
        private readonly IGithubBranchesStore _githubBranchesStore;

        public HarborWebhookService(IGithubBranchesStore githubBranchesStore)
        {
            _githubBranchesStore = githubBranchesStore;
        }

        public async Task HandleWebhookAsync(HarborWebhookPayload payload)
        {
            if (payload.Type != HarborWebhookType.DELETE_ARTIFACT)
            {
                // do nothing
                return;
            }
            var allGithubBranches = new List<GithubBranch>();
            foreach (var resource in payload.EventData.Resources)
            {
                var githubBranches = await _githubBranchesStore.GetAsync(new GithubBranchFilter
                {
                    HelmChart = resource.ResourceUrl,
                    IsDeleted = false,
                });

                if (githubBranches.Any())
                {
                    foreach (var githubBranch in githubBranches)
                    {
                        githubBranch.HelmChart = null;
                    }

                }
                allGithubBranches.AddRange(githubBranches);
            }
            await _githubBranchesStore.UpdateAsync(allGithubBranches);
         }
    }
}
