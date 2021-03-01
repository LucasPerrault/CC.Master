using Lucca.Core.Api.Abstractions.Paging;

namespace Instances.Application.Demos
{
    public class DemoListQuery
    {
        public IPageToken Page { get; set; } = null;
        public bool IsActive { get; set; } = true;
    }
}
