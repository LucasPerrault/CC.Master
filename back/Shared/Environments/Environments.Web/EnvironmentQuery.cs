using Environments.Domain.Storage;
using Lucca.Core.Api.Abstractions.Paging;
using System.Collections.Generic;
using Tools;
using Tools.Web;

namespace Environments.Web
{
    public class EnvironmentQuery
    {
        public IPageToken Page { get; set; } = null;
        public string Subdomain { get; set; } = null;
        public string Search { get; set; } = null;
        public HashSet<bool> IsActive { get; set; } = new HashSet<bool> { true };

        public EnvironmentFilter ToFilter()
        {
            return new EnvironmentFilter
            {
                Subdomain = string.IsNullOrEmpty(Subdomain)  ? CompareString.Bypass : CompareString.Equals(Subdomain),
                Search = Search,
                IsActive = IsActive.ToCompareBoolean()
            };
        }
    }
}
