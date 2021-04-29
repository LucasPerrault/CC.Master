using Lucca.Core.Api.Abstractions.Paging;
using System.Collections.Generic;

namespace Instances.Application.Demos
{
    public class DemoListQuery
    {
        public IPageToken Page { get; set; } = null;
        public HashSet<bool> IsActive { get; set; } = new HashSet<bool> { true };
    }
}
