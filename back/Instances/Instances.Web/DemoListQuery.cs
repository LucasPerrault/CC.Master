using Instances.Domain.Demos.Filtering;
using Lucca.Core.Api.Abstractions.Paging;
using System.Collections.Generic;
using Tools;
using Tools.Web;

namespace Instances.Web
{
    public class DemoListQuery
    {
        public IPageToken Page { get; set; } = null;
        public HashSet<bool> IsActive { get; set; } = new HashSet<bool> { true };

        public DemoFilter ToDemoFilter()
        {
            return new DemoFilter
            {
                IsActive = IsActive.ToBoolCombination(),
            };
        }
    }
}
