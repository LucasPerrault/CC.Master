using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instances.Application.Specflow.Tests.Shared.Tooling
{
    public class DummyQueryPager : IQueryPager
    {
        public IQueryable<T> GetPagedQueryable<T>(IQueryable<T> unpagedQuery, IPageToken token) where T : class
        {
            return unpagedQuery;
        }

        public async Task<Page<T>> ToPageAsync<T>(IQueryable<T> unpagedQuery, IPageToken token) where T : class
        {
            var items = await unpagedQuery.ToListAsync();
            return new Page<T>
            {
                Items = items,
                Count = items.Count,
                Prev = null,
                Next = null,
            };
        }
    }
}
