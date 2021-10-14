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
        private readonly Mock<IPreviewConfigurationsRepository> _previewConfigurationsRepositoryMock;
        private readonly Mock<IGithubService> _githubServiceMock;
        private readonly GithubBranchesRepository _githubBranchesRepository;

        public GithubBranchesRepositoryTests()
        {
            _githubBranchesStoreMock = new(MockBehavior.Strict);
            _previewConfigurationsRepositoryMock = new(MockBehavior.Strict);
            _githubServiceMock = new(MockBehavior.Strict);

            _githubBranchesRepository = new GithubBranchesRepository(
                _githubBranchesStoreMock.Object,
                _previewConfigurationsRepositoryMock.Object,
                _githubServiceMock.Object
            );
        }

        #region CreateAsync
        [Fact]
        public async Task CreateAsync()
        {
            var githubBranch = new GithubBranch();
            _githubBranchesStoreMock
                .Setup(gb => gb.CreateAsync(It.IsAny<GithubBranch>()))
                .Returns<GithubBranch>(b => Task.FromResult(b));
            _previewConfigurationsRepositoryMock
                .Setup(pc => pc.CreateByBranchAsync(It.IsAny<GithubBranch>()))
                .Returns(Task.CompletedTask);

            var result = await _githubBranchesRepository.CreateAsync(githubBranch);

            result.Should().Be(githubBranch);
            _githubBranchesStoreMock.Verify(gb => gb.CreateAsync(githubBranch));
            _previewConfigurationsRepositoryMock.Verify(pc => pc.CreateByBranchAsync(githubBranch));
        }
        #endregion

        #region CreateAsync_Multipleparams
        [Fact]
        public async Task CreateAsync_MultiParams()
        {
            var codeSources = new List<CodeSource>()
            {
                new CodeSource()
            };
            var githubBranch = new GithubBranch()
            {
                Name = "main",
                CodeSources = codeSources,
                LastPushedAt = DateTime.MinValue
            };
            _githubBranchesStoreMock
                .Setup(gb => gb.GetFirstAsync(It.IsAny<GithubBranchFilter>()))
                .ReturnsAsync((GithubBranch)null);
            _githubBranchesStoreMock
                .Setup(gb => gb.CreateAsync(It.IsAny<GithubBranch>()))
                .Returns<GithubBranch>(b => Task.FromResult(b));
            _previewConfigurationsRepositoryMock
                .Setup(pc => pc.CreateByBranchAsync(It.IsAny<GithubBranch>()))
                .Returns(Task.CompletedTask);

            var result = await _githubBranchesRepository.CreateAsync(codeSources, "main", new GithubApiCommit());

            result.Should().BeEquivalentTo(githubBranch);
            _githubBranchesStoreMock.Verify(gb => gb.CreateAsync(It.IsAny<GithubBranch>()));
        }
        #endregion

        #region GetNonDeletedBranchByNameAsync
        [Fact]
        public async Task GetNonDeletedBranchByNameAsync()
        {
            var codeSource = new CodeSource { Id = 42 };
            var name = "myBranch";
            _githubBranchesStoreMock
                .Setup(gb => gb.GetFirstAsync(It.IsAny<GithubBranchFilter>()))
                .ReturnsAsync(new GithubBranch());

            await _githubBranchesRepository.GetNonDeletedBranchByNameAsync(codeSource, name);

            _githubBranchesStoreMock.Verify(gb => gb.GetFirstAsync(It.Is<GithubBranchFilter>(g =>
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
