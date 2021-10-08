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
    public class GithubBranchesRepositoryTests
    {
        private readonly Mock<IGithubBranchesStore> _mockGithubBranchesStore;
        private readonly GithubBranchesRepository _githubBranchesRepository;

        public GithubBranchesRepositoryTests()
        {
            _mockGithubBranchesStore = new(MockBehavior.Strict);
            _githubBranchesRepository = new GithubBranchesRepository(_mockGithubBranchesStore.Object);
        }

        #region CreateAsync
        [Fact]
        public async Task CreateAsync()
        {
            var githubBranch = new GithubBranch();
            _mockGithubBranchesStore
                .Setup(gb => gb.CreateAsync(It.IsAny<GithubBranch>()))
                .Returns<GithubBranch>(b => Task.FromResult(b));

            var result = await _githubBranchesRepository.CreateAsync(githubBranch);

            result.Should().Be(githubBranch);
            _mockGithubBranchesStore.Verify(gb => gb.CreateAsync(githubBranch));
        }
        #endregion

        #region GetNonDeletedBranchByNameAsync
        [Fact]
        public async Task GetNonDeletedBranchByNameAsync()
        {
            var codeSource = new CodeSource { Id = 42 };
            var name = "myBranch";
            _mockGithubBranchesStore
                .Setup(gb => gb.GetFirstAsync(It.IsAny<GithubBranchFilter>()))
                .ReturnsAsync(new GithubBranch());

            await _githubBranchesRepository.GetNonDeletedBranchByNameAsync(codeSource, name);

            _mockGithubBranchesStore.Verify(gb => gb.GetFirstAsync(It.Is<GithubBranchFilter>(g =>
                g.CodeSourceId == 42 &&
                g.IsDeleted == false &&
                g.Name == "myBranch"
            )));
        }
        #endregion

        #region UpdateAsync
        [Fact]
        public async Task UpdateAsync()
        {
            var githubBranch = new GithubBranch();
            _mockGithubBranchesStore
                .Setup(gb => gb.UpdateAsync(It.IsAny<GithubBranch>()))
                .Returns<GithubBranch>(b => Task.FromResult(b));

            var result = await _githubBranchesRepository.UpdateAsync(githubBranch);

            result.Should().Be(githubBranch);
            _mockGithubBranchesStore.Verify(gb => gb.UpdateAsync(githubBranch));
        }
        #endregion
    }
}
