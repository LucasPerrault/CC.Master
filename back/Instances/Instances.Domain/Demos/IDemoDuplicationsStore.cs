using Lucca.Core.Api.Abstractions.Paging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Instances.Domain.Demos
{

    public interface IDemoDuplicationsStore
    {
        Task<Page<DemoDuplication>> GetAsync(IPageToken token, params Expression<Func<DemoDuplication, bool>>[] filters);
        Task<IQueryable<DemoDuplication>> GetAsync(Expression<Func<DemoDuplication, bool>> filter);
        Task<DemoDuplication> CreateAsync(DemoDuplication demo);
        IQueryable<DemoDuplication> GetAll();
    }
}