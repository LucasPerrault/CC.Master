using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Web.Controllers.Locks;
using Lock.Web;
using Lucca.Core.Api.Abstractions.Fields;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Abstractions.Sorting;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Web.Controllers
{
    [ApiController, Route("/api/cafe/sync")]
    public class SyncController
    {
        private readonly ISyncService _syncService;

        public SyncController(ISyncService syncService)
        {
            _syncService = syncService;
        }

        [HttpPost("huge")]
        [OneRequestAtATime(AdvancedFiltersSyncLock.Name, AdvancedFiltersSyncLock.TimeoutInSeconds)]
        [ForbidIfMissing(Operation.SyncAllCafe)]
        public Task HugeSyncAsync()
        {
            return _syncService.SyncEverythingAsync();
        }

        [HttpPost("multi-tenant")]
        [OneRequestAtATime(AdvancedFiltersSyncLock.Name, AdvancedFiltersSyncLock.TimeoutInSeconds)]
        [ForbidIfMissing(Operation.SyncAllCafe)]
        public Task MultiSyncAsync()
        {
            return _syncService.SyncMultiTenantDataAsync();
        }

        [HttpPost("mono-tenant")]
        [OneRequestAtATime(AdvancedFiltersSyncLock.Name, AdvancedFiltersSyncLock.TimeoutInSeconds)]
        [ForbidIfMissing(Operation.SyncAllCafe)]
        public Task MonoSyncAsync([FromQuery]SubdomainSyncQuery query)
        {
            if (!query.Subdomain.Any())
            {
                throw new BadRequestException("Subdomain query param is mandatory");
            }
            return _syncService.SyncMonoTenantDataAsync(query.Subdomain);
        }

        [HttpPost("mono-tenant/random")]
        [OneRequestAtATime(AdvancedFiltersSyncLock.Name, AdvancedFiltersSyncLock.TimeoutInSeconds)]
        [ForbidIfMissing(Operation.SyncAllCafe)]
        public Task RandomMonoSyncAsync([FromQuery]int tenantCount)
        {
            if (tenantCount == 0)
            {
                throw new BadRequestException("tenantCount query param is mandatory");
            }
            return _syncService.SyncRandomMonoTenantDataAsync(tenantCount);
        }

        [HttpPost("mono-tenant/range")]
        [OneRequestAtATime(AdvancedFiltersSyncLock.Name, AdvancedFiltersSyncLock.TimeoutInSeconds)]
        [ForbidIfMissing(Operation.SyncAllCafe)]
        public Task RangeMonoSyncAsync([FromQuery]RangeSyncQuery syncQuery)
        {
            IPageToken pageToken = new NumberPageToken
            (
                SortingQuery.FromRawValue
                (
                    nameof(Environment.Id)
                ),
                syncQuery.Page,
                syncQuery.Limit,
                RootFields.None
            );
            const int maxLimit = 500;
            if (pageToken.Limit > maxLimit)
            {
                throw new BadRequestException($"Max limit is {maxLimit}");
            }
            return _syncService.SyncRangeMonoTenantDataAsync(pageToken);
        }
    }

    public class SubdomainSyncQuery
    {
        public HashSet<string> Subdomain { get; set; } = new HashSet<string>();
    }

    public class RangeSyncQuery
    {
        public int Limit { get; set; } = 1;
        public int Page { get; set; } = 1;
    }

}
