using Cache.Abstractions;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Filtering;
using Instances.Domain.Instances.Models;
using Moq;
using Rights.Domain.Filtering;
using System;
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
            demoStoreMock
                .Setup(ds => ds.GetAsync(It.IsAny<DemoFilter>(), It.IsAny<AccessRight>()))
                .ReturnsAsync(new List<Demo>());
            var cacheServiceMock = new Mock<ICacheService>();
            cacheServiceMock.Setup(cs => cs.GetAsync<string>(It.IsAny<CacheKey<string>>())).ReturnsAsync("my-cached-cluster");
            var clusterSelector = new ClusterSelector(config, demoStoreMock.Object, cacheServiceMock.Object);

            Assert.Equal(config.FixedClusterName, await clusterSelector.GetFillingClusterAsync("toto"));
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
            demoStoreMock
                .Setup(ds => ds.GetAsync(It.IsAny<DemoFilter>(), It.IsAny<AccessRight>()))
                .ReturnsAsync(new List<Demo>());
            var cacheServiceMock = new Mock<ICacheService>();
            cacheServiceMock.Setup(cs => cs.GetAsync<string>(It.IsAny<CacheKey<string>>())).ReturnsAsync("my-cached-cluster");
            var clusterSelector = new ClusterSelector(config, demoStoreMock.Object, cacheServiceMock.Object);

            Assert.NotEqual(config.FixedClusterName, await clusterSelector.GetFillingClusterAsync("toto"));
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
            demoStoreMock
                .Setup(ds => ds.GetAsync(It.IsAny<DemoFilter>(), It.IsAny<AccessRight>()))
                .ReturnsAsync(new List<Demo>());
            var cachedCluster = "my-cached-cluster";
            var cacheServiceMock = new Mock<ICacheService>();
            cacheServiceMock.Setup(cs => cs.GetAsync<string>(It.IsAny<CacheKey<string>>())).ReturnsAsync(cachedCluster);
            var clusterSelector = new ClusterSelector(config, demoStoreMock.Object, cacheServiceMock.Object);

            Assert.Equal(cachedCluster, await clusterSelector.GetFillingClusterAsync("toto"));
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

            demoStoreMock
                .Setup(ds => ds.GetAsync(It.IsAny<DemoFilter>(), It.IsAny<AccessRight>()))
                .ReturnsAsync(new List<Demo>());
            var cacheServiceMock = new Mock<ICacheService>();
            string cachedCluster = null;
            cacheServiceMock.Setup(cs => cs.GetAsync<string>(It.IsAny<CacheKey<string>>())).ReturnsAsync(cachedCluster);
            var clusterSelector = new ClusterSelector(config, demoStoreMock.Object, cacheServiceMock.Object);

            Assert.Equal(leastUsedClusterName, await clusterSelector.GetFillingClusterAsync("toto"));
            demoStoreMock.Verify(ds => ds.GetNumberOfActiveDemosByCluster(), Times.Once);
        }

        [Fact]
        public async Task GetFillingClusterShouldReturnOldestPreviousDemoClusterInPriority()
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

            demoStoreMock
                .Setup(ds => ds.GetAsync(It.IsAny<DemoFilter>(), It.IsAny<AccessRight>()))
                .ReturnsAsync(new List<Demo>
                {
                    new Demo { CreatedAt = new DateTime(2020, 01, 01), Cluster = "too-old-to-be-used"},
                    new Demo { CreatedAt = new DateTime(2021, 01, 01), Cluster = "use-me"},
                });
            var cacheServiceMock = new Mock<ICacheService>();
            string cachedCluster = null;
            cacheServiceMock.Setup(cs => cs.GetAsync(It.IsAny<CacheKey<string>>())).ReturnsAsync(cachedCluster);
            var clusterSelector = new ClusterSelector(config, demoStoreMock.Object, cacheServiceMock.Object);

            Assert.Equal("use-me", await clusterSelector.GetFillingClusterAsync("toto"));
            demoStoreMock.Verify(ds => ds.GetNumberOfActiveDemosByCluster(), Times.Never);
        }
    }
}
