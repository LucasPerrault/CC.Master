using FluentAssertions;
using Instances.Application.Instances;
using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using Lucca.Core.Shared.Domain.Exceptions;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
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

        #region GetAllReleasesAsync
        [Fact]
        public async Task StableWithoutCodeSource_GetAllReleaseAsync()
        {
            var codeSourceEmailApi = new CodeSource { Code = "EmailApi", GithubRepo = "https://github.com/LuccaSA/Lucca.Emails" };
            var codeSourceEmailWorker = new CodeSource { Code = "EmailWorker", GithubRepo = "https://github.com/LuccaSA/Lucca.Emails" };
            var codeSourceFiggo = new CodeSource { Code = "Figgo", GithubRepo = "https://github.com/LuccaSA/Figgo" };
            var githubBranchEmailApi = new GithubBranch { Id = 1, HelmChart = "http://myHelmEmail", Name = "myProductionBranch" };
            var githubBranchEmailWorker = new GithubBranch { Id = 2, HelmChart = "http://myHelmEmail", Name = "myProductionBranch2" };
            var githubBranchFiggo = new GithubBranch { Id = 3, HelmChart = "http://myFiggo", Name = "figgoTempBranch" };

            _githubBranchesStoreMock
                .Setup(g => g.GetProductionBranchesAsync(It.IsAny<IEnumerable<CodeSource>>()))
                .ReturnsAsync(new Dictionary<CodeSource, GithubBranch>
                {
                    { codeSourceEmailApi, githubBranchEmailApi },
                    { codeSourceEmailWorker, githubBranchEmailWorker },
                    { codeSourceFiggo, githubBranchFiggo },
                });

            var result = await _helmRepository.GetAllReleasesAsync(null, null, true);

            result.Should().HaveCount(2);

            result.First().GitRef.Should().Be(githubBranchEmailWorker.Name);

            _githubBranchesStoreMock.Verify(
                g => g.GetProductionBranchesAsync(It.Is<IEnumerable<CodeSource>>(c => c == null))
            );
        }

        [Fact]
        public async Task StableWithCodeSource_GetAllReleaseAsync()
        {
            var codeSourceEmailApi = new CodeSource { Code = "EmailApi", GithubRepo = "https://github.com/LuccaSA/Lucca.Emails" };
            var codeSourceEmailWorker = new CodeSource { Code = "EmailWorker", GithubRepo = "https://github.com/LuccaSA/Lucca.Emails" };
            var githubBranchEmailApi = new GithubBranch { Id = 1, HelmChart = "http://myHelmEmail", Name = "myProductionBranch" };
            var githubBranchEmailWorker = new GithubBranch { Id = 2, HelmChart = "http://myHelmEmail", Name = "myProductionBranch2" };

            CodeSourceFilter codeSourceFilterCaptured = null;
            _codeSourceStoreMock
                .Setup(g => g.GetAsync(It.IsAny<CodeSourceFilter>()))
                .Returns<CodeSourceFilter>(c =>
                {
                    codeSourceFilterCaptured = c;
                    return Task.FromResult(new List<CodeSource>
                    {
                        codeSourceEmailApi,
                        codeSourceEmailWorker
                    });
                });

            _githubBranchesStoreMock
                .Setup(g => g.GetProductionBranchesAsync(It.IsAny<IEnumerable<CodeSource>>()))
                .ReturnsAsync(new Dictionary<CodeSource, GithubBranch>
                {
                    { codeSourceEmailApi, githubBranchEmailApi },
                    { codeSourceEmailWorker, githubBranchEmailWorker }
                });

            var result = await _helmRepository.GetAllReleasesAsync("Lucca.Emails", null, true);

            result.Should().HaveCount(1);
            result.First().GitRef.Should().Be(githubBranchEmailWorker.Name);
            codeSourceFilterCaptured.Should().NotBeNull();
            codeSourceFilterCaptured.GithubRepo.Should().Be("https://github.com/LuccaSA/Lucca.Emails");

            _githubBranchesStoreMock.Verify(
                g => g.GetProductionBranchesAsync(It.Is<IEnumerable<CodeSource>>(c => c.Contains(codeSourceEmailApi) && c.Contains(codeSourceEmailWorker)))
            );
        }

        [Fact]
        public async Task GitRef_GetAllReleaseAsync()
        {
            var codeSourceEmailApi = new CodeSource { Code = "EmailApi", GithubRepo = "https://github.com/LuccaSA/Lucca.Emails" };
            var codeSourceEmailWorker = new CodeSource { Code = "EmailWorker", GithubRepo = "https://github.com/LuccaSA/Lucca.Emails" };
            var githubBranchEmailApi = new GithubBranch { Id = 1, HelmChart = "http://myHelmEmail", Name = "myProductionBranch" };
            var githubBranchEmailWorker = new GithubBranch { Id = 2, HelmChart = "http://myHelmEmail", Name = "myProductionBranch2" };

            GithubBranchFilter githubBranchFilterCaptured = null;
            _githubBranchesStoreMock
                .Setup(g => g.GetAsync(It.IsAny<GithubBranchFilter>()))
                .Returns<GithubBranchFilter>(b =>
                {
                    githubBranchFilterCaptured = b;
                    return Task.FromResult(new List<GithubBranch>
                    {
                        new GithubBranch
                        {
                            CodeSources = new List<CodeSource>
                            {
                                codeSourceEmailApi
                            }
                        }
                    });
                });

            var result = await _helmRepository.GetAllReleasesAsync(null, "myBranch", false);

            result.Should().HaveCount(1);

            githubBranchFilterCaptured.Should().NotBeNull();
            githubBranchFilterCaptured.HasHelmChart.Should().BeTrue();
            githubBranchFilterCaptured.Name.Should().Be("myBranch");
        }
        #endregion
    }
}
