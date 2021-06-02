using Cache.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Domain.Demos
{
    public interface IClusterSelector
    {
        Task<string> GetFillingClusterAsync();
    }

    public class ClusterSelectorConfiguration
    {
        public bool UseFixedCluster { get; set; }
        public string FixedClusterName { get; set; }
        public int ClusterChoiceCacheRetentionInHours { get; set; }
    }


    public class ClusterSelectorCacheKey : CacheKey<string>
    {

        public override string Key => $"demos:selectedCluster";
    }

    public class ClusterSelector : IClusterSelector
    {
        private readonly ClusterSelectorConfiguration _configuration;
        private readonly IDemosStore _demosStore;
        private readonly ICacheService _cacheService;

        public ClusterSelector(ClusterSelectorConfiguration configuration, IDemosStore demosStore, ICacheService cacheService)
        {
            _configuration = configuration;
            _demosStore = demosStore;
            _cacheService = cacheService;
        }

        public async Task<string> GetFillingClusterAsync()
        {
            if(_configuration.UseFixedCluster)
            {
                return _configuration.FixedClusterName;
            }
            var clusterChoiceCacheKey = new ClusterSelectorCacheKey();
            var cachedClusterChoice = await _cacheService.GetAsync(clusterChoiceCacheKey);
            if(cachedClusterChoice != null)
            {
                return cachedClusterChoice;
            }

            var clusterChoice = await GetSelectedClusterByCountAsync();

            await _cacheService.SetAsync
            (
                clusterChoiceCacheKey,
                clusterChoice,
                CacheInvalidation.After(TimeSpan.FromHours(_configuration.ClusterChoiceCacheRetentionInHours))
            );
            return clusterChoice;
        }

        private async Task<string> GetSelectedClusterByCountAsync()
        {
            var counts = await _demosStore.GetNumberOfActiveDemosByCluster();
           return counts.OrderBy(c => c.Value).FirstOrDefault().Key;
        }
    }
}
