using Lucca.Core.Api.Abstractions.Paging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Instances.Domain.Demos
{
    public interface IDemosStore
    {
        Task<Page<Demo>> GetAsync(IPageToken token, params Expression<Func<Demo, bool>>[] filters);
    }
}
