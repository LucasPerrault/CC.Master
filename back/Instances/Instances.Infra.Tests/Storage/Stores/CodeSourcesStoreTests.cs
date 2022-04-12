using FluentAssertions;
using Instances.Domain.CodeSources;
using Instances.Infra.Storage;
using Instances.Infra.Storage.Stores;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testing.Infra;
using Xunit;

namespace Instances.Infra.Tests.Storage.Stores
{
    public class CodeSourcesStoreTests
    {
        private readonly InstancesDbContext _instancesDbContext;
        private readonly Mock<IQueryPager> _queryPagerMock;
        private readonly CodeSourcesStore _codeSourcesStore;

        public CodeSourcesStoreTests()
        {
            _instancesDbContext = InMemoryDbHelper.InitialiseDb<InstancesDbContext>("Instances", o => new InstancesDbContext(o));
            _queryPagerMock = new Mock<IQueryPager>();
            _queryPagerMock
                .Setup(p => p.ToPageAsync(It.IsAny<IQueryable<CodeSource>>(), It.IsAny<IPageToken>()))
                .Returns<IQueryable<CodeSource>, IPageToken>(
                    (queryable, pageToken) => Task.FromResult(new Page<CodeSource> { Items = queryable.ToList() })
                );

            _codeSourcesStore = new CodeSourcesStore(_instancesDbContext, _queryPagerMock.Object);
        }

        #region ReplaceProductionArtifactsAsync
        [Fact]
        public async Task ReplaceProductionArtifactsAsync_NewEntries()
        {
            var codeSource = new CodeSource { Id = 2 };
            await _instancesDbContext.AddAsync(new CodeSource { Id = 1, Lifecycle = CodeSourceLifecycleStep.InProduction });
            await _instancesDbContext.AddAsync(new CodeSource { Id = codeSource.Id, Lifecycle = CodeSourceLifecycleStep.InProduction });
            await _instancesDbContext.AddAsync(new CodeSourceArtifacts { ArtifactType = CodeSourceArtifactType.BackZip, ArtifactUrl = new System.Uri("http://blbl.local"), CodeSourceId = 1 });
            await _instancesDbContext.SaveChangesAsync();

            await _codeSourcesStore.ReplaceProductionArtifactsAsync(codeSource, new List<CodeSourceArtifacts>
            {
                new CodeSourceArtifacts { ArtifactType = CodeSourceArtifactType.AnonymizationScript, FileName = "abcd", ArtifactUrl = new System.Uri("http://google.fr") },
                new CodeSourceArtifacts { ArtifactType = CodeSourceArtifactType.AnonymizationScript, FileName = "cdef", ArtifactUrl = new System.Uri("http://google2.fr") }
            });

            _instancesDbContext.Set<CodeSourceArtifacts>().Count().Should().Be(3);
            _instancesDbContext.Set<CodeSourceArtifacts>().Where(c => c.CodeSourceId == codeSource.Id).Count().Should().Be(2);
        }

        [Fact]
        public async Task ReplaceProductionArtifactsAsync_ReplaceAll()
        {
            var codeSource = new CodeSource { Id = 2 };
            await _instancesDbContext.AddAsync(new CodeSource { Id = 1, Lifecycle = CodeSourceLifecycleStep.InProduction });
            await _instancesDbContext.AddAsync(new CodeSource { Id = codeSource.Id, Lifecycle = CodeSourceLifecycleStep.InProduction });
            await _instancesDbContext.AddAsync(new CodeSourceArtifacts { ArtifactType = CodeSourceArtifactType.BackZip, ArtifactUrl = new System.Uri("http://blbl.local"), CodeSourceId = 1 });
            await _instancesDbContext.AddAsync(new CodeSourceArtifacts { ArtifactType = CodeSourceArtifactType.BackZip, ArtifactUrl = new System.Uri("http://old.local"), CodeSourceId = codeSource.Id });
            await _instancesDbContext.SaveChangesAsync();

            await _codeSourcesStore.ReplaceProductionArtifactsAsync(codeSource, new List<CodeSourceArtifacts>
            {
                new CodeSourceArtifacts { ArtifactType = CodeSourceArtifactType.AnonymizationScript, FileName = "abcd", ArtifactUrl = new System.Uri("http://google.fr") },
                new CodeSourceArtifacts { ArtifactType = CodeSourceArtifactType.AnonymizationScript, FileName = "cdef", ArtifactUrl = new System.Uri("http://google2.fr") }
            });

            _instancesDbContext.Set<CodeSourceArtifacts>().Count().Should().Be(3);
            _instancesDbContext.Set<CodeSourceArtifacts>().Where(c => c.CodeSourceId == codeSource.Id).Count().Should().Be(2);

            _instancesDbContext
                .Set<CodeSourceArtifacts>()
                .Where(c => c.CodeSourceId == codeSource.Id)
                .Select(c => c.FileName)
                .Should()
                .Contain("abcd", "cdef");
        }

        #endregion
    }
}
