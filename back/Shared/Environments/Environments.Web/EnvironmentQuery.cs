using Environments.Domain.Storage;
using Lucca.Core.Api.Abstractions.Paging;
using System.Collections.Generic;
using System.Linq;
using Tools;
using Tools.Web;

namespace Environments.Web
{
    public class EnvironmentQuery
    {
        public HashSet<int> Id { get; set; } = new HashSet<int>();
        public IPageToken Page { get; set; } = null;
        public HashSet<string> Subdomain { get; set; } = new HashSet<string>();
        public string Search { get; set; } = null;
        public HashSet<bool> IsActive { get; set; } = new HashSet<bool> { true };

        public EnvironmentFilter ToFilter()
        {
            return new EnvironmentFilter
            {
                Subdomain = Subdomain.Any() ? CompareString.Equals(Subdomain) : CompareString.Bypass,
                Search = Search,
                IsActive = IsActive.ToCompareBoolean(),
                Ids = Id
            };
        }
    }
}
