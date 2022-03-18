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

        private readonly Mock<IGithubBranchesStore> _githubBranchesStoreMock;
        private readonly Mock<IGithubReposStore> _githubReposStoreMock;

        public HelmRepositoryTests()
        {
            _githubBranchesStoreMock = new(MockBehavior.Strict);
            _githubReposStoreMock = new(MockBehavior.Strict);


            _helmRepository = new HelmRepository(
                _githubBranchesStoreMock.Object,
                _githubReposStoreMock.Object
            );
        }

        #region CreateHelmAsync
        [Fact]
        public async Task NotFound_CreateHelmAsync()
        {
            var releaseName = "MyRepo";
            var branchName = "myBranch";
            var helmChart = "helmChart";

            _githubReposStoreMock
                .Setup(g => g.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync((GithubRepo)null);

            Func<Task> act = () => _helmRepository.CreateHelmAsync(releaseName, branchName, helmChart);

            await act.Should().ThrowAsync<BadRequestException>();

            _githubReposStoreMock.Verify(c => c.GetByUriAsync(new Uri("https://github.com/LuccaSA/MyRepo")));
        }

        [Fact]
        public async Task Ok_CreateHelmAsync()
        {
            var releaseName = "MyRepo";
            var branchName = "myBranch";
            var helmChart = "helmChart";

            var githubBranch = new GithubBranch();
            GithubBranchFilter githubBranchFilterCaught = null;

            _githubReposStoreMock
                .Setup(g => g.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync(new GithubRepo());
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

            _githubReposStoreMock.Verify(g => g.GetByUriAsync(It.IsAny<Uri>()));
            _githubBranchesStoreMock.Verify(g => g.GetAsync(It.IsAny<GithubBranchFilter>()));
            _githubBranchesStoreMock.Verify(g => g.UpdateAsync(It.IsAny<IEnumerable<GithubBranch>>()));

            githubBranch.HelmChart.Should().Be(helmChart);
            githubBranchFilterCaught.Should().NotBeNull();
            githubBranchFilterCaught.Name.Should().Be(branchName);
            githubBranchFilterCaught.IsDeleted.Should().Be(Tools.CompareBoolean.FalseOnly);
        }

        #endregion

        #region GetAllReleasesAsync
        [Fact]
        public async Task GetAllReleasesAsync_AllStables()
        {
            var repo1 = new GithubRepo { Id = 1, Name = "repo1", Url = new Uri("https://github.com/LuccaSA/repo2") };
            var repo2 = new GithubRepo { Id = 2, Name = "repo2", Url = new Uri("https://github.com/LuccaSA/repo2") };

            var codeSource1_1 = new CodeSource { Id = 1, Name = "code1.1", Repo = repo1, RepoId = repo1.Id };
            var codeSource1_2 = new CodeSource { Id = 2, Name = "code1.2", Repo = repo1, RepoId = repo1.Id };
            var codeSource2_1 = new CodeSource { Id = 3, Name = "code2.1", Repo = repo2, RepoId = repo2.Id };

            var githubBranch1_1 = new GithubBranch { Id = 1, Repo = repo1, RepoId = repo1.Id, Name = "branch1", CreatedAt = DateTime.Now, HelmChart = "abc" }; // prod codeSource 1_1
            var githubBranch1_2 = new GithubBranch { Id = 2, Repo = repo1, RepoId = repo1.Id, Name = "branch2", CreatedAt = DateTime.Now, HelmChart = "def" }; // nothing
            var githubBranch1_3 = new GithubBranch { Id = 3, Repo = repo1, RepoId = repo1.Id, Name = "branch3", CreatedAt = DateTime.Now, HelmChart = "ght", }; // prod codeSource 1_2
            var githubBranch2_1 = new GithubBranch { Id = 4, Repo = repo2, RepoId = repo2.Id, Name = "branch1", CreatedAt = DateTime.Now, HelmChart = "ijk" }; // prod codeSource 2_1

            _githubBranchesStoreMock
                .Setup(g => g.GetProductionBranchesAsync(It.IsAny<GithubBranchFilter>()))
                .ReturnsAsync(new List<GithubBranch>
                {
                    githubBranch1_1,
                    githubBranch2_1
                });
            _githubReposStoreMock
                .Setup(g => g.GetAllAsync())
                .ReturnsAsync(new List<GithubRepo> { repo1, repo2 });
            _githubBranchesStoreMock
                .Setup(g => g.GetAsync(It.IsAny<GithubBranchFilter>()))
                .ReturnsAsync(new List<GithubBranch>());

            var releases = await _helmRepository.GetAllReleasesAsync(HelmRequest.ForAllRepos());

            releases.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllReleasesAsync_ForRepo()
        {
            var repo1 = new GithubRepo { Id = 1, Name = "repo1", Url = new Uri("https://github.com/LuccaSA/repo2") };

            var codeSource1_1 = new CodeSource { Id = 1, Name = "code1.1", Repo = repo1, RepoId = repo1.Id };

            var githubBranch1_1 = new GithubBranch { Id = 1, Repo = repo1, RepoId = repo1.Id, Name = "branch1", CreatedAt = DateTime.Now }; // prod codeSource 1_1

            _githubReposStoreMock
                .Setup(g => g.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync(repo1);
            _githubBranchesStoreMock
                .Setup(g => g.GetAsync(It.IsAny<GithubBranchFilter>()))
                .ReturnsAsync(new List<GithubBranch>() { githubBranch1_1 });

            var releases = await _helmRepository.GetAllReleasesAsync(HelmRequest.ForRepo(repo1.Name, githubBranch1_1.Name, shouldBeStable: false));

            releases.Should().HaveCount(1);
        }
        #endregion
    }
}
