using FluentAssertions;
using Instances.Application.Webhooks.Harbor;
using Instances.Application.Webhooks.Harbor.Models;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Application.Tests.Webhooks.Harbor
{
    public class HarborWebhookServiceTests
    {
        private readonly HarborWebhookService _harborWebhookService;
        private readonly Mock<IGithubBranchesStore> _githubBranchesStoreMock;

        public HarborWebhookServiceTests()
        {
            _githubBranchesStoreMock = new(MockBehavior.Strict);

            _harborWebhookService = new HarborWebhookService(_githubBranchesStoreMock.Object);
        }

        [Fact]
        public async Task HandleWebhookAsync()
        {
            var githubBranch = new GithubBranch()
            {
                HelmChart = "myChart"
            };
            _githubBranchesStoreMock
                .Setup(g => g.GetAsync(It.IsAny<GithubBranchFilter>()))
                .ReturnsAsync(new List<GithubBranch> { githubBranch });
            _githubBranchesStoreMock
                .Setup(g => g.UpdateAsync(It.IsAny<IEnumerable<GithubBranch>>()))
                .Returns(Task.CompletedTask);

            await _harborWebhookService.HandleWebhookAsync(new HarborWebhookPayload
            {
                Type = HarborWebhookType.DELETE_ARTIFACT,
                OccurAt = 1634560355420,
                Operator = "admin",
                EventData = new HarborWebhookEventData
                {
                    Repository = new HarborWebhookRepository
                    {
                        DateCreated = 1634560355420,
                        Name = "cleemy-procurment",
                        Namespace = "lucca/helm",
                        RepoFullName = "lucca/helm/cleemy-procurement",
                        RepoType = "private"
                    },
                    Resources = new List<HarborWebhookResource>
                    {
                        new HarborWebhookResource
                        {
                            Digest = "sha256:8a9e9863dbb6e10edb5adfe917c00da84e1700fa76e7ed02476aa6e6fb8ee0d8",
                            Tag = "latest",
                            ResourceUrl = "hub.harbor.com/lucca/helm/cleemy-procurement:latest"
                        }
                    }
                }
            });


            _githubBranchesStoreMock
                .Verify(g => g.UpdateAsync(It.IsAny<IEnumerable<GithubBranch>>()));

            githubBranch.HelmChart.Should().BeNull();
        }

    }
}
