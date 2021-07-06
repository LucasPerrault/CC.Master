using FluentAssertions;
using Instances.Application.CodeSources;
using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Infra.Storage;
using Instances.Infra.Storage.Models;
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

        private readonly CodeSourcesRepository _codeSourcesRepository;

        public CodeSourcesRepositoryTests()
        {
            _githubBranchesStoreMock = new Mock<IGithubBranchesStore>();
            _queryPagerMock = new Mock<IQueryPager>();
            _queryPagerMock
                .Setup(p => p.ToPageAsync(It.IsAny<IQueryable<StoredCodeSource>>(), It.IsAny<IPageToken>()))
                .Returns<IQueryable<StoredCodeSource>, IPageToken>(
                    (queryable, pageToken) => Task.FromResult(new Page<StoredCodeSource> { Items = queryable.ToList() })
                );
            _fetcherServiceMock = new Mock<ICodeSourceFetcherService>();
            _instancesDbContext = InMemoryDbHelper.InitialiseDb<InstancesDbContext>("Instances", o => new InstancesDbContext(o));
            _codeSourceBuildUrlServiceMock = new Mock<ICodeSourceBuildUrlService>(MockBehavior.Strict);

            _codeSourcesRepository = new CodeSourcesRepository(
                new CodeSourcesStore(_instancesDbContext, _queryPagerMock.Object),
                _githubBranchesStoreMock.Object,
                _fetcherServiceMock.Object,
                _codeSourceBuildUrlServiceMock.Object
            );
        }

        [Fact]
        public async Task ShouldFilterOnLifeCycle()
        {
            await _instancesDbContext.AddAsync(new StoredCodeSource { Id = 1, Lifecycle = CodeSourceLifecycleStep.Preview });
            await _instancesDbContext.AddAsync(new StoredCodeSource  { Id = 2, Lifecycle = CodeSourceLifecycleStep.Referenced });
            await _instancesDbContext.SaveChangesAsync();
            var filter = new CodeSourceFilter {Lifecycle = new HashSet<CodeSourceLifecycleStep> { CodeSourceLifecycleStep.Referenced }};

            var codeSources = await _codeSourcesRepository.GetAsync(new NumberPageToken(),filter);

            codeSources.Items.Single().Id.Should().Be(2);
        }

        [Fact]
        public async Task ShouldFilterOutDeletedLifeCycleByDefault()
        {
            await _instancesDbContext.AddAsync(new StoredCodeSource { Id = 1, Lifecycle = CodeSourceLifecycleStep.Deleted });
            await _instancesDbContext.AddAsync(new StoredCodeSource { Id = 2, Lifecycle = CodeSourceLifecycleStep.ToDelete });

            var activeSources = new List<StoredCodeSource>
            {
                new StoredCodeSource { Id = 3, Lifecycle = CodeSourceLifecycleStep.ReadyForDeploy },
                new StoredCodeSource { Id = 4, Lifecycle = CodeSourceLifecycleStep.Preview },
                new StoredCodeSource { Id = 5, Lifecycle = CodeSourceLifecycleStep.Referenced },
                new StoredCodeSource { Id = 6, Lifecycle = CodeSourceLifecycleStep.InProduction },
            };
            await _instancesDbContext.AddRangeAsync(activeSources);
            await _instancesDbContext.SaveChangesAsync();

            var filter = new CodeSourceFilter { Lifecycle = CodeSource.ActiveSteps };

            var codeSources = await _codeSourcesRepository.GetAsync(new NumberPageToken(),filter);

            codeSources.Items.Count().Should().Be(4);
            codeSources.Items.Should().NotContain(a => a.Lifecycle == CodeSourceLifecycleStep.Deleted);
            codeSources.Items.Should().NotContain(a => a.Lifecycle == CodeSourceLifecycleStep.ToDelete);
        }

        [Fact]
        public async Task ShouldCallFetcherWhenFetchingRepo()
        {
            var repoUrl = "https://github.com/aperture-science/glados";

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
            await _instancesDbContext.AddAsync(new StoredCodeSource { Id = 1, Lifecycle = CodeSourceLifecycleStep.Referenced });
            await _instancesDbContext.SaveChangesAsync();

            await _codeSourcesRepository.UpdateAsync(1, new CodeSourceUpdate { Lifecycle = CodeSourceLifecycleStep.InProduction });

            _instancesDbContext.Set<StoredCodeSource>().Single().Lifecycle.Should().Be(CodeSourceLifecycleStep.InProduction);
        }

        [Fact]
        public async Task ShouldUpdateProdVersion()
        {
            await _instancesDbContext.AddAsync(new StoredCodeSource { Id = 1, Code = "source-code"});
            await _instancesDbContext.SaveChangesAsync();

            await _codeSourcesRepository.UpdateProductionVersionAsync(new CodeSourceProductionVersionDto
            {
                CodeSourceCode = "source-code",
                BranchName = "main-branch-in-production"
            });

            var source = (await _codeSourcesRepository.GetAsync(new NumberPageToken(),CodeSourceFilter.ById(1))).Items.SingleOrDefault();

            source.CurrentProductionVersion.BranchName.Should().Be("main-branch-in-production");
        }

        [Fact]
        public async Task ShouldMarkAsInProd()
        {
            await _instancesDbContext.AddAsync(new StoredCodeSource { Id = 1, Code = "source-code"});
            await _instancesDbContext.SaveChangesAsync();

            await _codeSourcesRepository.UpdateProductionVersionAsync(new CodeSourceProductionVersionDto
            {
                CodeSourceCode = "source-code",
                BranchName = "main-branch-in-production"
            });

            var source = (await _codeSourcesRepository.GetAsync(new NumberPageToken(), CodeSourceFilter.ById(1))).Items.SingleOrDefault();
            source.Lifecycle.Should().Be(CodeSourceLifecycleStep.InProduction);
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

        [Fact]
        public async Task ShouldCreateDefaultBranchAfterSourceCreation()
        {
            var source = new CodeSource();

            await _codeSourcesRepository.CreateAsync(source);

            _githubBranchesStoreMock.Verify(s => s.CreateForNewSourceCodeAsync(source), Times.Once);
        }

        #region GetBuildUrlAsync
        [Fact]
        public async Task GetBuildUrlAsync_Ok()
        {
            var branchName = "branchName";
            var buildNumber = "1234";
            var code = "testSoft";
            var expectedResult = "http://test.test.com";
            await _instancesDbContext.AddAsync(new StoredCodeSource { Id = 1, Code = code, JenkinsProjectUrl = "http://google.fr", Lifecycle = CodeSourceLifecycleStep.Referenced });
            await _instancesDbContext.SaveChangesAsync();

            _codeSourceBuildUrlServiceMock
                .Setup(c => c.IsValidBuildNumber(It.IsAny<string>()))
                .Returns(true);
            _codeSourceBuildUrlServiceMock
                .Setup(c => c.GenerateBuildUrl(It.IsAny<CodeSource>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(expectedResult);

            var result = await _codeSourcesRepository.GetBuildUrlAsync(code, branchName, buildNumber);

            result.Should().Be(expectedResult);
            _codeSourceBuildUrlServiceMock.Verify(c => c.IsValidBuildNumber(buildNumber));
            _codeSourceBuildUrlServiceMock.Verify(c => c.GenerateBuildUrl(It.Is<CodeSource>(c => c.Code == code), branchName, buildNumber));
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
            await _instancesDbContext.AddAsync(new StoredCodeSource { Id = 1, Code = code, Lifecycle = CodeSourceLifecycleStep.Referenced });
            await _instancesDbContext.SaveChangesAsync();

            _codeSourceBuildUrlServiceMock
                .Setup(c => c.IsValidBuildNumber(It.IsAny<string>()))
                .Returns(false);

            Func<Task> act = () => _codeSourcesRepository.GetBuildUrlAsync(code, branchName, buildNumber);

            await act.Should().ThrowAsync<BadRequestException>();
            _codeSourceBuildUrlServiceMock.Verify(c => c.IsValidBuildNumber(buildNumber));
        }

        #endregion

    }
}
