using FluentAssertions;
using Instances.Application.Instances;
using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using Lucca.Core.Shared.Domain.Exceptions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Application.Tests.Instances
{
    public class HelmRepositoryTests
    {
        private readonly HelmRepository _helmRepository;

        private readonly Mock<ICodeSourcesStore> _codeSourceStoreMock;
        private readonly Mock<IGithubBranchesStore> _githubBranchesStoreMock;

        public HelmRepositoryTests()
        {
            _codeSourceStoreMock = new(MockBehavior.Strict);
            _githubBranchesStoreMock = new(MockBehavior.Strict);

            _helmRepository = new HelmRepository(
                _codeSourceStoreMock.Object,
                _githubBranchesStoreMock.Object
            );
        }

        #region CreateHelmAsync
        [Fact]
        public async Task NotFound_CreateHelmAsync()
        {
            var releaseName = "MyRepo";
            var branchName = "myBranch";
            var helmChart = "helmChart";

            CodeSourceFilter codeSourceFilterCaught = null;
            _codeSourceStoreMock
                .Setup(c => c.GetAsync(It.IsAny<CodeSourceFilter>()))
                .Returns<CodeSourceFilter>(c =>
                {
                    codeSourceFilterCaught = c;
                    return Task.FromResult(new List<CodeSource>());
                });

            Func<Task> act = () => _helmRepository.CreateHelmAsync(releaseName, branchName, helmChart);

            await act.Should().ThrowAsync<BadRequestException>();

            _codeSourceStoreMock.Verify(c => c.GetAsync(It.IsAny<CodeSourceFilter>()));
            codeSourceFilterCaught.Should().NotBeNull();
            codeSourceFilterCaught.GithubRepo.Should().Be("https://github.com/LuccaSA/" + releaseName);
        }

        [Fact]
        public async Task Ok_CreateHelmAsync()
        {
            var releaseName = "MyRepo";
            var branchName = "myBranch";
            var helmChart = "helmChart";

            var githubBranch = new GithubBranch();
            GithubBranchFilter githubBranchFilterCaught = null;

            _codeSourceStoreMock
                .Setup(c => c.GetAsync(It.IsAny<CodeSourceFilter>()))
                .ReturnsAsync(new List<CodeSource> { new CodeSource() });
            _githubBranchesStoreMock
                .Setup(gb => gb.GetAsync(It.IsAny<GithubBranchFilter>()))
                .Returns<GithubBranchFilter>(g =>
                {
                    githubBranchFilterCaught = g;
                    return Task.FromResult(new List<GithubBranch> { githubBranch });
                });
            _githubBranchesStoreMock
                .Setup(gb => gb.UpdateAsync(It.IsAny<IEnumerable<GithubBranch>>()))
                .Returns(Task.CompletedTask);

            await _helmRepository.CreateHelmAsync(releaseName, branchName, helmChart);

            _codeSourceStoreMock.Verify(c => c.GetAsync(It.IsAny<CodeSourceFilter>()));
            _githubBranchesStoreMock.Verify(g => g.GetAsync(It.IsAny<GithubBranchFilter>()));
            _githubBranchesStoreMock.Verify(g => g.UpdateAsync(It.IsAny<IEnumerable<GithubBranch>>()));

            githubBranch.HelmChart.Should().Be(helmChart);
            githubBranchFilterCaught.Should().NotBeNull();
            githubBranchFilterCaught.Name.Should().Be(branchName);
            githubBranchFilterCaught.IsDeleted.Should().BeFalse();
        }

        #endregion
    }
}
