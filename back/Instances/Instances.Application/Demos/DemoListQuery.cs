using Instances.Domain.Demos.Filtering;
using Lucca.Core.Api.Abstractions.Paging;
using System.Collections.Generic;
using Tools;

namespace Instances.Application.Demos
{
    public class DemoListQuery
    {
        public IPageToken Page { get; set; } = null;
        public HashSet<bool> IsActive { get; set; } = new HashSet<bool> { true };

        public DemoFilter ToDemoFilter(DemoAccess access)
        {
            return new DemoFilter(access)
            {
                IsActive = IsActive.ToBoolCombination(),
            };
        }
    }
}
