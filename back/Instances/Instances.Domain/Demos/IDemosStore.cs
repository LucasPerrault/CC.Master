using Instances.Domain.Demos.Filtering;
using Lucca.Core.Api.Abstractions.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Domain.Demos
{
    public interface IDemosStore
    {
        Task<IQueryable<Demo>> GetAsync(DemoFilter filter);
        Task<Page<Demo>> GetAsync(IPageToken pageToken, DemoFilter filter);
        Task<Dictionary<string, int>> GetNumberOfActiveDemosByCluster();
        Task<Demo> CreateAsync(Demo demo);
        Task UpdateDeletionScheduleAsync(Demo demo, DateTime deletionScheduledOn);
        Task DeleteAsync(Demo demo);
        Task<Demo> GetActiveByIdAsync(int id, DemoAccess access);
    }
}
