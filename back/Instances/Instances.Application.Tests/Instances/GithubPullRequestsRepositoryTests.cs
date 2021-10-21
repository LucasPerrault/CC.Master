using FluentAssertions;
using Instances.Application.Instances;
using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Application.Tests.Instances
{
    public class GithubPullRequestsRepositoryTests
    {
        private readonly Mock<IGithubPullRequestsStore> _mockGithubPullRequestsStore;
        private readonly GithubPullRequestsRepository _githubBranchesRepository;

        public GithubPullRequestsRepositoryTests()
        {
            _mockGithubPullRequestsStore = new(MockBehavior.Strict);
            _githubBranchesRepository = new GithubPullRequestsRepository(_mockGithubPullRequestsStore.Object);
        }

        #region CreateAsync
        [Fact]
        public async Task CreateAsync()
        {
            var githubPullRequest = new GithubPullRequest();
            _mockGithubPullRequestsStore
                .Setup(gb => gb.CreateAsync(It.IsAny<GithubPullRequest>()))
                .Returns<GithubPullRequest>(b => Task.FromResult(b));

            var result = await _githubBranchesRepository.CreateAsync(githubPullRequest);

            result.Should().Be(githubPullRequest);
            _mockGithubPullRequestsStore.Verify(gb => gb.CreateAsync(githubPullRequest));
        }
        #endregion

        #region GetNonDeletedBranchByNameAsync
        [Fact]
        public async Task GetByNumberAsync()
        {
            var codeSource = new CodeSource { Id = 42 };
            var pullRequestNumber = 123;
            _mockGithubPullRequestsStore
                .Setup(gb => gb.GetFirstAsync(It.IsAny<GithubPullRequestFilter>()))
                .ReturnsAsync(new GithubPullRequest());

            await _githubBranchesRepository.GetByNumberAsync(codeSource, pullRequestNumber);

            _mockGithubPullRequestsStore.Verify(gb => gb.GetFirstAsync(It.Is<GithubPullRequestFilter>(g =>
                g.CodeSourceId == 42 &&
                g.Number == pullRequestNumber
            )));
        }
        #endregion

        #region UpdateAsync
        [Fact]
        public async Task UpdateAsync()
        {
            var githubPullRequest = new GithubPullRequest();
            _mockGithubPullRequestsStore
                .Setup(gb => gb.UpdateAsync(It.IsAny<GithubPullRequest>()))
                .Returns<GithubPullRequest>(b => Task.FromResult(b));

            var result = await _githubBranchesRepository.UpdateAsync(githubPullRequest);

            result.Should().Be(githubPullRequest);
            _mockGithubPullRequestsStore.Verify(gb => gb.UpdateAsync(githubPullRequest));
        }
        #endregion
    }
}
