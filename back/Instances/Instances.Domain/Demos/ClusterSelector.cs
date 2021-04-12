using Cache.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Domain.Demos
{
    public interface IClusterSelector
    {
        Task<string> GetFillingCluster();
    }

    public class ClusterSelectorConfiguration
    {
        public bool UseFixedCluster { get; set; }
        public string FixedClusterName { get; set; }
        public int ClusterChoiceCacheRetentionInHours { get; set; }
    }


    public class ClusterSelectorCacheKey : CacheKey<string>
    {
        private int _cacheDurationInHours;

        public override string Key => $"demos:selectedCluster";
        public override TimeSpan? Invalidation => TimeSpan.FromHours(_cacheDurationInHours);

        public ClusterSelectorCacheKey(int cacheDurationInHours)
        {
            _cacheDurationInHours = cacheDurationInHours;
        }
    }

    public class ClusterSelector : IClusterSelector
    {
        private ClusterSelectorConfiguration _configuration;
        private IDemosStore _demosStore;
        private ICacheService _cacheService;

        public ClusterSelector(ClusterSelectorConfiguration configuration, IDemosStore demosStore, ICacheService cacheService)
        {
            _configuration = configuration;
            _demosStore = demosStore;
            _cacheService = cacheService;
        }

        public async Task<string> GetFillingCluster()
        {
            if(_configuration.UseFixedCluster)
            {
                return _configuration.FixedClusterName;
            }
            var clusterChoiceCacheKey = new ClusterSelectorCacheKey(_configuration.ClusterChoiceCacheRetentionInHours);
            var cachedClusterChoice = await _cacheService.GetAsync(clusterChoiceCacheKey);
            if(cachedClusterChoice != null)
            {
                return cachedClusterChoice;
            }

            var clusterChoice = await GetSelectedClusterByCountAsync();

            await _cacheService.SetAsync(clusterChoiceCacheKey, clusterChoice);
            return clusterChoice;
        }

        private async Task<string> GetSelectedClusterByCountAsync()
        {
            var counts = await _demosStore.GetNumberOfActiveDemosByCluster();
           return counts.OrderBy(c => c.Value).FirstOrDefault().Key;
        }
    }
}
