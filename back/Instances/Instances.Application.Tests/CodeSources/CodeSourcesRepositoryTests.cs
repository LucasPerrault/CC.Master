using FluentAssertions;
using Instances.Application.CodeSources;
using Instances.Application.Instances;
using Instances.Application.Instances.Dtos;
using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Domain.Github;
using Instances.Domain.Github.Models;
using Instances.Infra.Storage;
using Instances.Infra.Storage.Stores;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Lucca.Core.Shared.Domain.Exceptions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testing.Infra;
using Xunit;

namespace Instances.Application.Tests.CodeSources
{
    public class CodeSourcesRepositoryTests
    {
        private readonly Mock<ICodeSourceFetcherService> _fetcherServiceMock;
        private readonly InstancesDbContext _instancesDbContext;
        private readonly Mock<IGithubBranchesStore> _githubBranchesStoreMock;
        private readonly Mock<IQueryPager> _queryPagerMock;
        private readonly Mock<ICodeSourceBuildUrlService> _codeSourceBuildUrlServiceMock;
        private readonly Mock<IArtifactsService> _artifactsServiceMock;
        private readonly Mock<IGithubService> _githubServiceMock;
        private readonly Mock<IPreviewConfigurationsRepository> _previewConfigurationsRepositoryMock;
        private readonly Mock<IGithubReposStore> _githubReposStoreMock;

        private readonly CodeSourcesRepository _codeSourcesRepository;

        public CodeSourcesRepositoryTests()
        {
            _githubBranchesStoreMock = new Mock<IGithubBranchesStore>(MockBehavior.Strict);
            _queryPagerMock = new Mock<IQueryPager>();
            _queryPagerMock
                .Setup(p => p.ToPageAsync(It.IsAny<IQueryable<CodeSource>>(), It.IsAny<IPageToken>()))
                .Returns<IQueryable<CodeSource>, IPageToken>(
                    (queryable, pageToken) => Task.FromResult(new Page<CodeSource> { Items = queryable.ToList() })
                );
            _fetcherServiceMock = new Mock<ICodeSourceFetcherService>();
            _instancesDbContext = InMemoryDbHelper.InitialiseDb<InstancesDbContext>("Instances", o => new InstancesDbContext(o));
            _codeSourceBuildUrlServiceMock = new Mock<ICodeSourceBuildUrlService>(MockBehavior.Strict);
            _artifactsServiceMock = new Mock<IArtifactsService>(MockBehavior.Strict);
            _githubServiceMock = new Mock<IGithubService>(MockBehavior.Strict);
            _previewConfigurationsRepositoryMock = new Mock<IPreviewConfigurationsRepository>(MockBehavior.Strict);
            _githubReposStoreMock = new(MockBehavior.Strict);

            _codeSourcesRepository = new CodeSourcesRepository(
                new CodeSourcesStore(_instancesDbContext, _queryPagerMock.Object),
                _githubBranchesStoreMock.Object,
                _fetcherServiceMock.Object,
                _codeSourceBuildUrlServiceMock.Object,
                _artifactsServiceMock.Object,
                _githubServiceMock.Object,
                _previewConfigurationsRepositoryMock.Object,
                _githubReposStoreMock.Object
            );
        }

        [Fact]
        public async Task ShouldFilterOnLifeCycle()
        {
            var githubRepo = (await _instancesDbContext.AddAsync(new GithubRepo { Name = "test", Url = new Uri("https://github.com/LuccaSA/test") })).Entity;
            await _instancesDbContext.AddAsync(new CodeSource { Id = 1, Repo = githubRepo, Lifecycle = CodeSourceLifecycleStep.Preview });
            await _instancesDbContext.AddAsync(new CodeSource { Id = 2, Repo = githubRepo, Lifecycle = CodeSourceLifecycleStep.Referenced });
            await _instancesDbContext.SaveChangesAsync();
            var filter = new CodeSourceFilter { Lifecycle = new HashSet<CodeSourceLifecycleStep> { CodeSourceLifecycleStep.Referenced } };

            var codeSources = await _codeSourcesRepository.GetAsync(new NumberPageToken(), filter);

            codeSources.Items.Single().Id.Should().Be(2);
        }

        [Fact]
        public async Task ShouldFilterOutDeletedLifeCycleByDefault()
        {
            var githubRepo = (await _instancesDbContext.AddAsync(new GithubRepo { Name = "test", Url = new Uri("https://github.com/LuccaSA/test") })).Entity;
            await _instancesDbContext.AddAsync(new CodeSource { Id = 1, Repo = githubRepo, Lifecycle = CodeSourceLifecycleStep.Deleted });
            await _instancesDbContext.AddAsync(new CodeSource { Id = 2, Repo = githubRepo, Lifecycle = CodeSourceLifecycleStep.ToDelete });

            var activeSources = new List<CodeSource>
            {
                new CodeSource { Id = 3, Repo = githubRepo, Lifecycle = CodeSourceLifecycleStep.ReadyForDeploy },
                new CodeSource { Id = 4, Repo = githubRepo, Lifecycle = CodeSourceLifecycleStep.Preview },
                new CodeSource { Id = 5, Repo = githubRepo, Lifecycle = CodeSourceLifecycleStep.Referenced },
                new CodeSource { Id = 6, Repo = githubRepo, Lifecycle = CodeSourceLifecycleStep.InProduction },
            };
            await _instancesDbContext.AddRangeAsync(activeSources);
            await _instancesDbContext.SaveChangesAsync();

            var filter = new CodeSourceFilter { Lifecycle = CodeSource.ActiveSteps };

            var codeSources = await _codeSourcesRepository.GetAsync(new NumberPageToken(), filter);

            codeSources.Items.Count().Should().Be(4);
            codeSources.Items.Should().NotContain(a => a.Lifecycle == CodeSourceLifecycleStep.Deleted);
            codeSources.Items.Should().NotContain(a => a.Lifecycle == CodeSourceLifecycleStep.ToDelete);
        }

        [Fact]
        public async Task ShouldCallFetcherWhenFetchingRepo()
        {
            var repoUrl = new Uri("https://github.com/aperture-science/glados");

            await _codeSourcesRepository.FetchFromRepoAsync(repoUrl);

            _fetcherServiceMock.Verify(s => s.FetchAsync(repoUrl), Times.Once);
        }

        [Fact]
        public async Task ShouldThrowWhenUpdatingNonExistingSource()
        {
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _codeSourcesRepository.UpdateAsync(1, new CodeSourceUpdate { Lifecycle = CodeSourceLifecycleStep.InProduction }));
            ex.Message.Should().Contain("Unknown code source");
        }

        [Fact]
        public async Task ShouldUpdateExisting()
        {
            var githubRepo = (await _instancesDbContext.AddAsync(new GithubRepo { Name = "test", Url = new Uri("https://github.com/LuccaSA/test") })).Entity;
            await _instancesDbContext.AddAsync(new CodeSource { Id = 1, Lifecycle = CodeSourceLifecycleStep.Referenced, Repo = githubRepo });
            await _instancesDbContext.SaveChangesAsync();

            await _codeSourcesRepository.UpdateAsync(1, new CodeSourceUpdate { Lifecycle = CodeSourceLifecycleStep.InProduction });

            _instancesDbContext.Set<CodeSource>().Single().Lifecycle.Should().Be(CodeSourceLifecycleStep.InProduction);
        }

        [Fact]
        public async Task ShouldUpdateProdVersion()
        {
            var listArtifacts = new List<CodeSourceArtifacts>
            {
                new CodeSourceArtifacts { Id = 1 }
            };
            var githubRepo = (await _instancesDbContext.AddAsync(new GithubRepo { Name = "test", Url = new Uri("https://github.com/LuccaSA/test") })).Entity;
            await _instancesDbContext.AddAsync(new CodeSource { Id = 1, Code = "source-code", Repo = githubRepo });
            await _instancesDbContext.SaveChangesAsync();
            _artifactsServiceMock
                .Setup(a => a.GetArtifactsAsync(It.IsAny<CodeSource>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(listArtifacts);

            await _codeSourcesRepository.UpdateProductionVersionAsync(new CodeSourceProductionVersionDto
            {
                CodeSourceCode = "source-code",
                BranchName = "main-branch-in-production",
                JenkinsBuildNumber = 12
            });

            var source = (await _codeSourcesRepository.GetAsync(new NumberPageToken(), CodeSourceFilter.ById(1))).Items.SingleOrDefault();

            source.CurrentProductionVersion.BranchName.Should().Be("main-branch-in-production");
            _instancesDbContext.Set<CodeSourceArtifacts>().Count().Should().Be(1);
            _artifactsServiceMock.Verify(a => a.GetArtifactsAsync(
                It.Is<CodeSource>(c => c.Id == 1),
                "main-branch-in-production",
                12)
            );
        }

        [Fact]
        public async Task ShouldMarkAsInProd()
        {
            var listArtifacts = new List<CodeSourceArtifacts>
            {
                new CodeSourceArtifacts { Id = 1 }
            };
            _artifactsServiceMock
                .Setup(a => a.GetArtifactsAsync(It.IsAny<CodeSource>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(listArtifacts);

            var githubRepo = (await _instancesDbContext.AddAsync(new GithubRepo { Name = "test", Url = new Uri("https://github.com/LuccaSA/test") })).Entity;
            await _instancesDbContext.AddAsync(new CodeSource { Id = 1, Code = "source-code", Repo = githubRepo });
            await _instancesDbContext.SaveChangesAsync();

            await _codeSourcesRepository.UpdateProductionVersionAsync(new CodeSourceProductionVersionDto
            {
                CodeSourceCode = "source-code",
                BranchName = "main-branch-in-production",
                JenkinsBuildNumber = 12
            });

            var source = (await _codeSourcesRepository.GetAsync(new NumberPageToken(), CodeSourceFilter.ById(1))).Items.SingleOrDefault();
            source.Lifecycle.Should().Be(CodeSourceLifecycleStep.InProduction);

            _instancesDbContext.Set<CodeSourceArtifacts>().Count().Should().Be(1);
            _artifactsServiceMock.Verify(a => a.GetArtifactsAsync(
                It.Is<CodeSource>(c => c.Id == 1),
                "main-branch-in-production",
                12)
            );
        }

        [Fact]
        public async Task ShouldThrowWhenUpdatingProdVersionOfUnknownSource()
        {
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _codeSourcesRepository.UpdateProductionVersionAsync(new CodeSourceProductionVersionDto
            {
                CodeSourceCode = "source-code"
            }));
            ex.Message.Should().Contain("Unknown code source");
        }

        #region GetBuildUrlAsync
        [Fact]
        public async Task GetBuildUrlAsync_Ok()
        {
            var branchName = "branchName";
            var buildNumber = "1234";
            var code = "testSoft";
            var expectedResult = "http://test.test.com";
            var githubRepo = await _instancesDbContext.AddAsync(new GithubRepo { Name = "test", Url = new Uri("https://github.com/LuccaSA/test") });
            await _instancesDbContext.AddAsync(new CodeSource { Id = 1, Code = code, JenkinsProjectUrl = "http://google.fr", Lifecycle = CodeSourceLifecycleStep.Referenced, Repo = githubRepo.Entity });
            await _instancesDbContext.SaveChangesAsync();

            _codeSourceBuildUrlServiceMock
                .Setup(c => c.IsValidBuildNumber(It.IsAny<string>()))
                .Returns(true);
            _codeSourceBuildUrlServiceMock
                .Setup(c => c.GenerateBuildUrlAsync(It.IsAny<CodeSource>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedResult);

            var result = await _codeSourcesRepository.GetBuildUrlAsync(code, branchName, buildNumber);

            result.Should().Be(expectedResult);
            _codeSourceBuildUrlServiceMock.Verify(c => c.IsValidBuildNumber(buildNumber));
            _codeSourceBuildUrlServiceMock.Verify(c => c.GenerateBuildUrlAsync(It.Is<CodeSource>(c => c.Code == code), branchName, buildNumber));
        }

        [Fact]
        public async Task GetBuildUrlAsync_InvalidBranchName()
        {
            var branchName = "branchName";
            var buildNumber = "1234";
            var code = "testSoft";

            _codeSourceBuildUrlServiceMock
                .Setup(c => c.IsValidBuildNumber(It.IsAny<string>()))
                .Returns(false);

            Func<Task> act = () => _codeSourcesRepository.GetBuildUrlAsync(code, branchName, buildNumber);

            await act.Should().ThrowAsync<BadRequestException>();
            _codeSourceBuildUrlServiceMock.Verify(c => c.IsValidBuildNumber(buildNumber));
        }

        [Fact]
        public async Task GetBuildUrlAsync_InvalidProjectUrl()
        {
            var branchName = "branchName";
            var buildNumber = "1234";
            var code = "testSoft";
            await _instancesDbContext.AddAsync(new CodeSource { Id = 1, Code = code, Lifecycle = CodeSourceLifecycleStep.Referenced });
            await _instancesDbContext.SaveChangesAsync();

            _codeSourceBuildUrlServiceMock
                .Setup(c => c.IsValidBuildNumber(It.IsAny<string>()))
                .Returns(false);

            Func<Task> act = () => _codeSourcesRepository.GetBuildUrlAsync(code, branchName, buildNumber);

            await act.Should().ThrowAsync<BadRequestException>();
            _codeSourceBuildUrlServiceMock.Verify(c => c.IsValidBuildNumber(buildNumber));
        }

        #endregion

        #region GetArtifactsAsync
        [Fact]
        public async Task GetArtifactsAsync_Ok()
        {
            await _instancesDbContext.AddRangeAsync(
                new CodeSource { Id = 1, Code = "source-code-1" },
                new CodeSource { Id = 2, Code = "source-code-2" }
            );
            await _instancesDbContext.AddRangeAsync(
                new CodeSourceArtifacts { CodeSourceId = 1, ArtifactType = CodeSourceArtifactType.AnonymizationScript, FileName = "abc.txt", ArtifactUrl = "http://test.com/abc" },
                new CodeSourceArtifacts { CodeSourceId = 1, ArtifactType = CodeSourceArtifactType.CleanScript, FileName = "efg.txt", ArtifactUrl = "http://test.com/efg" },
                new CodeSourceArtifacts { CodeSourceId = 1, ArtifactType = CodeSourceArtifactType.BackZip, FileName = "hij.txt", ArtifactUrl = "http://test.com/hij" },
                new CodeSourceArtifacts { CodeSourceId = 2, ArtifactType = CodeSourceArtifactType.FrontZip, FileName = "klm.txt", ArtifactUrl = "http://test.com/klm" }
            );
            await _instancesDbContext.SaveChangesAsync();

            var artifacts1 = await _codeSourcesRepository.GetArtifactsAsync(1);
            var artifacts2 = await _codeSourcesRepository.GetArtifactsAsync(2);


            artifacts1.Should().HaveCount(3);
            artifacts2.Should().HaveCount(1);
        }
        #endregion GetArtifactsAsync

        #region CreateAsync
        [Fact]
        public async Task Existing_CreateAsync()
        {
            var githubRepoUrl = new Uri("https://github.com/LuccaSA/repo");
            var githubRepo = new GithubRepo { Id = 10, Name = "repo", Url = githubRepoUrl };

            await _instancesDbContext.AddAsync(githubRepo);
            await _instancesDbContext.AddAsync(new CodeSource { Id = 1, Lifecycle = CodeSourceLifecycleStep.InProduction, RepoId = 10 });
            await _instancesDbContext.SaveChangesAsync();

            _githubReposStoreMock
                .Setup(gr => gr.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync(githubRepo);
            _githubBranchesStoreMock
                .Setup(gb => gb.GetAsync(It.IsAny<GithubBranchFilter>()))
                .ReturnsAsync(new List<GithubBranch>() { new GithubBranch() });
            _previewConfigurationsRepositoryMock
                .Setup(pc => pc.CreateByBranchAsync(It.IsAny<IEnumerable<GithubBranch>>(), It.IsAny<CodeSource>()))
                .Returns(Task.CompletedTask);

            var createdCodeSource = await _codeSourcesRepository.CreateAsync(new CreateCodeSourceDto
            {
                RepoUrl = githubRepoUrl
            });

            createdCodeSource.Should().NotBeNull();
            createdCodeSource.RepoId.Should().Be(githubRepo.Id);
        }

        [Fact]
        public async Task New_CreateAsync()
        {
            var githubRepoUrl = new Uri("https://github.com/LuccaSA/repo");
            var branchName = "myBranch";

            _githubReposStoreMock
                .Setup(gr => gr.GetByUriAsync(It.IsAny<Uri>()))
                .ReturnsAsync((GithubRepo)null);
            _githubReposStoreMock
                .Setup(gr => gr.CreateAsync(It.IsAny<Uri>()))
                .ReturnsAsync(new GithubRepo
                {
                    Id = 10,
                    Url = githubRepoUrl
                });
            _githubServiceMock
                .Setup(gs => gs.GetBranchNamesAsync(It.IsAny<Uri>()))
                .ReturnsAsync(new List<string> { branchName });
            _githubServiceMock
                .Setup(gs => gs.GetGithubBranchHeadCommitInfoAsync(It.IsAny<Uri>(), It.IsAny<string>()))
                .ReturnsAsync(new GithubApiCommit());
            _githubBranchesStoreMock
                .Setup(gb => gb.CreateAsync(It.IsAny<IEnumerable<GithubBranch>>()))
                .ReturnsAsync(new List<GithubBranch>() );
            _githubBranchesStoreMock
                .Setup(gb => gb.GetAsync(It.IsAny<GithubBranchFilter>()))
                .ReturnsAsync(new List<GithubBranch> { new GithubBranch() });
            _previewConfigurationsRepositoryMock
                .Setup(pc => pc.CreateByBranchAsync(It.IsAny<IEnumerable<GithubBranch>>(), It.IsAny<CodeSource>()))
                .Returns(Task.CompletedTask);

            await _codeSourcesRepository.CreateAsync(new CreateCodeSourceDto
            {
                RepoUrl = githubRepoUrl
            });

            _githubServiceMock.Verify(gs => gs.GetGithubBranchHeadCommitInfoAsync(githubRepoUrl, branchName));
        }
        #endregion
    }
}
