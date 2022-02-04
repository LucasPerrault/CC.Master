using FluentAssertions;
using Instances.Application.Instances;
using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Application.Tests.Instances
{
    public class GithubBranchesRepositoryTests
    {
        private readonly Mock<IGithubBranchesStore> _githubBranchesStoreMock;
        private readonly Mock<IGithubReposStore> _githubReposStoreMock;
        private readonly Mock<IPreviewConfigurationsRepository> _previewConfigurationsRepositoryMock;
        private readonly Mock<IGithubService> _githubServiceMock;
        private readonly GithubBranchesRepository _githubBranchesRepository;

        public GithubBranchesRepositoryTests()
        {
            _githubBranchesStoreMock = new(MockBehavior.Strict);
            _githubReposStoreMock = new(MockBehavior.Strict);
            _previewConfigurationsRepositoryMock = new(MockBehavior.Strict);
            _githubServiceMock = new(MockBehavior.Strict);

            _githubBranchesRepository = new GithubBranchesRepository(
                _githubBranchesStoreMock.Object,
                _githubReposStoreMock.Object,
                _previewConfigurationsRepositoryMock.Object,
                _githubServiceMock.Object
            );
        }

        #region CreateAsync
        [Fact]
        public async Task CreateAsync()
        {
            var repoUri = new Uri("https://github.com/LuccaSA/test");
            var githubApiCommit = new GithubApiCommit();
            _githubReposStoreMock
                .Setup(gr => gr.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new GithubRepo
                {
                    Url = repoUri
                });
            _githubBranchesStoreMock
                .Setup(gb => gb.GetFirstAsync(It.IsAny<GithubBranchFilter>()))
                .ReturnsAsync((GithubBranch)null);
            _githubBranchesStoreMock
                .Setup(gb => gb.CreateAsync(It.IsAny<GithubBranch>()))
                .Returns<GithubBranch>(b => Task.FromResult(b));
            _previewConfigurationsRepositoryMock
                .Setup(pc => pc.CreateByBranchAsync(It.IsAny<GithubBranch>()))
                .Returns(Task.CompletedTask);

            var result = await _githubBranchesRepository.CreateAsync(10, "myBranch", githubApiCommit);

            result.Should().NotBeNull();
            result.Name.Should().Be("myBranch");
            result.RepoId.Should().Be(10);
            _githubBranchesStoreMock.Verify(gb => gb.CreateAsync(result));
        }
        #endregion

        #region CreateAsync_Multipleparams
        [Fact]
        public async Task CreateAsync_without_commit()
        {
            var repoUri = new Uri("https://github.com/LuccaSA/test");
            var githubApiCommit = new GithubApiCommit();
            _githubReposStoreMock
                .Setup(gr => gr.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new GithubRepo
                {
                    Url = repoUri
                });
            _githubServiceMock
                .Setup(gs => gs.GetGithubBranchHeadCommitInfoAsync(It.IsAny<Uri>(), It.IsAny<string>()))
                .ReturnsAsync(githubApiCommit);
            _githubBranchesStoreMock
                .Setup(gb => gb.GetFirstAsync(It.IsAny<GithubBranchFilter>()))
                .ReturnsAsync((GithubBranch)null);
            _githubBranchesStoreMock
                .Setup(gb => gb.CreateAsync(It.IsAny<GithubBranch>()))
                .Returns<GithubBranch>(b => Task.FromResult(b));
            _previewConfigurationsRepositoryMock
                .Setup(pc => pc.CreateByBranchAsync(It.IsAny<GithubBranch>()))
                .Returns(Task.CompletedTask);

            var result = await _githubBranchesRepository.CreateAsync(10, "myBranch", null);

            result.Should().NotBeNull();
            result.Name.Should().Be("myBranch");
            result.RepoId.Should().Be(10);
            _githubBranchesStoreMock.Verify(gb => gb.CreateAsync(result));
            _githubServiceMock.Verify(gs => gs.GetGithubBranchHeadCommitInfoAsync(repoUri, "myBranch"));
        }
        #endregion

        #region GetNonDeletedBranchByNameAsync
        [Fact]
        public async Task GetNonDeletedBranchByNameAsync()
        {
            var repoId = 42;
            var name = "myBranch";
            _githubBranchesStoreMock
                .Setup(gb => gb.GetFirstAsync(It.IsAny<GithubBranchFilter>()))
                .ReturnsAsync(new GithubBranch());

            await _githubBranchesRepository.GetNonDeletedBranchByNameAsync(repoId, name);

            _githubBranchesStoreMock.Verify(gb => gb.GetFirstAsync(It.Is<GithubBranchFilter>(g =>
                g.RepoIds.Contains(repoId) &&
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
            _githubBranchesStoreMock
                .Setup(gb => gb.UpdateAsync(It.IsAny<GithubBranch>()))
                .Returns<GithubBranch>(b => Task.FromResult(b));

            var result = await _githubBranchesRepository.UpdateAsync(githubBranch);

            result.Should().Be(githubBranch);
            _githubBranchesStoreMock.Verify(gb => gb.UpdateAsync(githubBranch));
        }
        #endregion
    }
}
