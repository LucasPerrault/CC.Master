using Environments.Domain.Storage;
using Lucca.Core.Api.Abstractions.Paging;
using Tools;

namespace Environments.Web
{
    public class EnvironmentQuery
    {
        public IPageToken Page { get; set; } = null;
        public string Subdomain { get; set; } = null;

        public EnvironmentFilter ToFilter()
        {
            return new EnvironmentFilter
            {
                Subdomain = string.IsNullOrEmpty(Subdomain)  ? CompareString.Bypass : CompareString.Equals(Subdomain),
            };
        }
    }
}
