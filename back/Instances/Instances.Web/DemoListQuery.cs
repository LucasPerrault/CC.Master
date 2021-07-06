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
        public string Search { get; set; } = null;
        public int? AuthorId { get; set; } = null;
        public int? DistributorId { get; set; } = null;
        public string Subdomain { get; set; } = null;
        public HashSet<bool> IsTemplate { get; set; } = new HashSet<bool>();
        public DemoListInstance Instance { get; set; } = new DemoListInstance();

        public DemoFilter ToDemoFilter()
        {
            return new DemoFilter
            {
                IsActive = IsActive.ToCompareBoolean(),
                Search = Search,
                IsTemplate = IsTemplate.ToCompareBoolean(),
                DistributorId = DistributorId,
                AuthorId = AuthorId,
                Subdomain = string.IsNullOrEmpty(Subdomain)  ? CompareString.Bypass : CompareString.Equals(Subdomain),
                IsProtected = Instance?.IsProtected?.ToCompareBoolean() ?? CompareBoolean.Bypass,
            };
        }
    }

    public class DemoListInstance
    {
        public HashSet<bool> IsProtected { get;set; } = new HashSet<bool>();
    }
}
