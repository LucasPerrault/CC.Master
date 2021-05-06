using Instances.Domain.Demos.Filtering;
using Lucca.Core.Api.Abstractions.Paging;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.Demos
{
    public interface IDemosStore
    {
        Task<List<Demo>> GetAsync(DemoFilter filter, AccessRight access);
        Task<Page<Demo>> GetAsync(IPageToken pageToken, DemoFilter filter, AccessRight access);
        Task<Demo> GetActiveByIdAsync(int id, AccessRight access);
        Task<Dictionary<string, int>> GetNumberOfActiveDemosByCluster();
        Task<Demo> CreateAsync(Demo demo);
        Task UpdateDeletionScheduleAsync(IEnumerable<DemoDeletionSchedule> schedules);
        Task UpdateCommentAsync(Demo demo, string comment);
        Task DeleteAsync(Demo demo);
    }

    public class DemoDeletionSchedule
    {
        public Demo Demo { get; set; }
        public DateTime DeletionScheduledOn { get; set; }
    }
}
