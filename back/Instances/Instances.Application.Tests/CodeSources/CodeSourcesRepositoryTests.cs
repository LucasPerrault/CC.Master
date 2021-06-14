using FluentAssertions;
using Instances.Application.CodeSources;
using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Infra.Storage;
using Instances.Infra.Storage.Models;
using Instances.Infra.Storage.Stores;
using Lucca.Core.Shared.Domain.Exceptions;
using Moq;
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

        public CodeSourcesRepositoryTests()
        {
            _fetcherServiceMock = new Mock<ICodeSourceFetcherService>();
            _instancesDbContext = InMemoryDbHelper.InitialiseDb<InstancesDbContext>("Instances", o => new InstancesDbContext(o));
        }

        [Fact]
        public async Task ShouldFilterOnLifeCycle()
        {
            await _instancesDbContext.AddAsync(new StoredCodeSource { Id = 1, Lifecycle = CodeSourceLifecycleStep.Preview });
            await _instancesDbContext.AddAsync(new StoredCodeSource  { Id = 2, Lifecycle = CodeSourceLifecycleStep.Referenced });
            await _instancesDbContext.SaveChangesAsync();

            var repository = new CodeSourcesRepository(new CodeSourcesStore(_instancesDbContext), _fetcherServiceMock.Object);
            var filter = new CodeSourceFilter {Lifecycle = new HashSet<CodeSourceLifecycleStep> { CodeSourceLifecycleStep.Referenced }};
            var codeSources = await repository.GetAsync(filter);
            codeSources.Single().Id.Should().Be(2);
        }

        [Fact]
        public async Task ShouldFilterOutDeletedLifeCycleByDefault()
        {
            await _instancesDbContext.AddAsync(new StoredCodeSource { Id = 1, Lifecycle = CodeSourceLifecycleStep.Deleted });

            var activeSources = new List<StoredCodeSource>
            {
                new StoredCodeSource { Id = 2, Lifecycle = CodeSourceLifecycleStep.ReadyForDeploy },
                new StoredCodeSource { Id = 3, Lifecycle = CodeSourceLifecycleStep.ToDelete },
                new StoredCodeSource { Id = 4, Lifecycle = CodeSourceLifecycleStep.Preview },
                new StoredCodeSource { Id = 5, Lifecycle = CodeSourceLifecycleStep.Referenced },
                new StoredCodeSource { Id = 6, Lifecycle = CodeSourceLifecycleStep.InProduction },
            };
            await _instancesDbContext.AddRangeAsync(activeSources);
            await _instancesDbContext.SaveChangesAsync();

            var repository = new CodeSourcesRepository
            (
                new CodeSourcesStore(_instancesDbContext),
                _fetcherServiceMock.Object
            );
            var filter = new CodeSourceFilter { Lifecycle = CodeSource.ActiveSteps };
            var codeSources = await repository.GetAsync(filter);
            codeSources.Count().Should().Be(5);
            codeSources.Should().NotContain(a => a.Lifecycle == CodeSourceLifecycleStep.Deleted);
        }

        [Fact]
        public async Task ShouldCallFetcherWhenFetchingRepo()
        {
            var repoUrl = "https://github.com/aperture-science/glados";
            var repository = new CodeSourcesRepository
            (
                new CodeSourcesStore(_instancesDbContext),
                _fetcherServiceMock.Object
            );
            await repository.FetchFromRepoAsync(repoUrl);
            _fetcherServiceMock.Verify(s => s.FetchAsync(repoUrl), Times.Once);
        }

        [Fact]
        public async Task ShouldThrowWhenUpdatingNonExistingSource()
        {
            var repository = new CodeSourcesRepository
            (
                new CodeSourcesStore(_instancesDbContext),
                _fetcherServiceMock.Object
            );
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => repository.UpdateAsync(1, new CodeSourceUpdate { Lifecycle = CodeSourceLifecycleStep.InProduction }));
            ex.Message.Should().Contain("Unknown code source");
        }

        [Fact]
        public async Task ShouldUpdateExisting()
        {
            await _instancesDbContext.AddAsync(new StoredCodeSource { Id = 1, Lifecycle = CodeSourceLifecycleStep.Referenced });
            await _instancesDbContext.SaveChangesAsync();
            var repository = new CodeSourcesRepository
            (
                new CodeSourcesStore(_instancesDbContext),
                _fetcherServiceMock.Object
            );

            await repository.UpdateAsync(1, new CodeSourceUpdate { Lifecycle = CodeSourceLifecycleStep.InProduction });
            _instancesDbContext.Set<StoredCodeSource>().Single().Lifecycle.Should().Be(CodeSourceLifecycleStep.InProduction);
        }

        [Fact]
        public async Task ShouldUpdateProdVersion()
        {
            await _instancesDbContext.AddAsync(new StoredCodeSource { Id = 1, Code = "source-code"});
            await _instancesDbContext.SaveChangesAsync();
            var repository = new CodeSourcesRepository
            (
                new CodeSourcesStore(_instancesDbContext),
                _fetcherServiceMock.Object
            );

            await repository.UpdateProductionVersionAsync(new CodeSourceProductionVersionDto
            {
                CodeSourceCode = "source-code",
                BranchName = "main-branch-in-production"
            });

            var source = (await repository.GetAsync(CodeSourceFilter.ById(1))).SingleOrDefault();
            source.CurrentProductionVersion.BranchName.Should().Be("main-branch-in-production");
        }

        [Fact]
        public async Task ShouldThrowWhenUpdatingProdVersionOfUnknownSource()
        {
            var repository = new CodeSourcesRepository
            (
                new CodeSourcesStore(_instancesDbContext),
                _fetcherServiceMock.Object
            );

            var ex = await Assert.ThrowsAsync<NotFoundException>(() => repository.UpdateProductionVersionAsync(new CodeSourceProductionVersionDto
            {
                CodeSourceCode = "source-code"
            }));
            ex.Message.Should().Contain("Unknown code source");
        }
    }
}
