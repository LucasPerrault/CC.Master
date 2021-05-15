using Cache.Abstractions;
using Instances.Domain.Demos;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Instances.Domain.Tests
{
    public class ClusterSelectorTest
    {
        [Fact]
        public async Task GetFillingClusterShouldReturnTheFixedClusterDefinedInConfigWhenTrueAsync()
        {
            var config = new ClusterSelectorConfiguration
            {
                ClusterChoiceCacheRetentionInHours = 1,
                FixedClusterName = "my-fixed-cluster",
                UseFixedCluster = true
            };

            var demoStoreMock = new Mock<IDemosStore>();
            demoStoreMock.Setup(ds => ds.GetNumberOfActiveDemosByCluster()).ReturnsAsync(new Dictionary<string, int>
            {
                { "my-fixed-cluster", 30000 },
                { "my-cluster-that-needs-to-be-filled", 1 }
            });
            var cacheServiceMock = new Mock<ICacheService>();
            cacheServiceMock.Setup(cs => cs.GetAsync<string>(It.IsAny<CacheKey<string>>())).ReturnsAsync("my-cached-cluster");
            var clusterSelector = new ClusterSelector(config, demoStoreMock.Object, cacheServiceMock.Object);

            Assert.Equal(config.FixedClusterName, await clusterSelector.GetFillingClusterAsync());
            demoStoreMock.Verify(ds => ds.GetNumberOfActiveDemosByCluster(), Times.Never);
        }

        [Fact]
        public async Task GetFillingClusterShouldNotReturnTheFixedClusterDefinedInConfigWhenFalseAsync()
        {
            var config = new ClusterSelectorConfiguration
            {
                ClusterChoiceCacheRetentionInHours = 1,
                FixedClusterName = "my-fixed-cluster",
                UseFixedCluster = false
            };

            var demoStoreMock = new Mock<IDemosStore>();
            demoStoreMock.Setup(ds => ds.GetNumberOfActiveDemosByCluster()).ReturnsAsync(new Dictionary<string, int>
            {
                { "my-fixed-cluster", 30000 },
                { "my-cluster-that-needs-to-be-filled", 1 }
            });
            var cacheServiceMock = new Mock<ICacheService>();
            cacheServiceMock.Setup(cs => cs.GetAsync<string>(It.IsAny<CacheKey<string>>())).ReturnsAsync("my-cached-cluster");
            var clusterSelector = new ClusterSelector(config, demoStoreMock.Object, cacheServiceMock.Object);

            Assert.NotEqual(config.FixedClusterName, await clusterSelector.GetFillingClusterAsync());
        }

        [Fact]
        public async Task GetFillingClusterShouldReturnTheCachedClusterWhenNotExpiredAsync()
        {
            var config = new ClusterSelectorConfiguration
            {
                ClusterChoiceCacheRetentionInHours = 1,
                FixedClusterName = "my-fixed-cluster",
                UseFixedCluster = false
            };

            var demoStoreMock = new Mock<IDemosStore>();
            demoStoreMock.Setup(ds => ds.GetNumberOfActiveDemosByCluster()).ReturnsAsync(new Dictionary<string, int>
            {
                { "my-fixed-cluster", 30000 },
                { "my-cluster-that-needs-to-be-filled", 1 }
            });
            var cachedCluster = "my-cached-cluster";
            var cacheServiceMock = new Mock<ICacheService>();
            cacheServiceMock.Setup(cs => cs.GetAsync<string>(It.IsAny<CacheKey<string>>())).ReturnsAsync(cachedCluster);
            var clusterSelector = new ClusterSelector(config, demoStoreMock.Object, cacheServiceMock.Object);

            Assert.Equal(cachedCluster, await clusterSelector.GetFillingClusterAsync());
            demoStoreMock.Verify(ds => ds.GetNumberOfActiveDemosByCluster(), Times.Never);
        }

        [Fact]
        public async Task GetFillingClusterShouldReturnTheLeastUsedClusterWhenNotCachedAsync()
        {
            var config = new ClusterSelectorConfiguration
            {
                ClusterChoiceCacheRetentionInHours = 1,
                FixedClusterName = "my-fixed-cluster",
                UseFixedCluster = false
            };

            var leastUsedClusterName = "my-cluster-that-needs-to-be-filled";
            var demoStoreMock = new Mock<IDemosStore>();
            demoStoreMock.Setup(ds => ds.GetNumberOfActiveDemosByCluster()).ReturnsAsync(new Dictionary<string, int>
            {
                { "my-fixed-cluster", 30000 },
                { leastUsedClusterName, 1 }
            });

            var cacheServiceMock = new Mock<ICacheService>();
            string cachedCluster = null;
            cacheServiceMock.Setup(cs => cs.GetAsync<string>(It.IsAny<CacheKey<string>>())).ReturnsAsync(cachedCluster);
            var clusterSelector = new ClusterSelector(config, demoStoreMock.Object, cacheServiceMock.Object);

            Assert.Equal(leastUsedClusterName, await clusterSelector.GetFillingClusterAsync());
            demoStoreMock.Verify(ds => ds.GetNumberOfActiveDemosByCluster(), Times.Once);
        }
    }
}
