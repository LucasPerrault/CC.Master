using Lucca.Core.Api.Abstractions.Paging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Instances.Domain.Demos
{
    public interface IDemosStore
    {
        Task<Page<Demo>> GetAsync(IPageToken token, params Expression<Func<Demo, bool>>[] filters);
        Task<IQueryable<Demo>> GetAsync(params Expression<Func<Demo, bool>>[] filters);
        Task<Demo> GetByInstanceIdAsync(int instanceId);
        Task<Demo> CreateAsync(Demo demo);
        Task<IQueryable<Demo>> GetActiveAsync(params Expression<Func<Demo, bool>>[] filters);
        Task DeleteAsync(Demo demo);
    }
}
