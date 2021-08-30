using System.Collections.Generic;
using Tools;

namespace Environments.Domain.Storage
{
    public class EnvironmentFilter : ValueObject
    {
        public CompareString Subdomain { get; set; } = CompareString.Bypass;
        public CompareBoolean IsActive { get; set; } = CompareBoolean.Bypass;

        public HashSet<int> Ids { get; set; } = new HashSet<int>();
        public string Search { get; set; }

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return Subdomain;
                yield return IsActive;
                yield return Search;

                foreach (var id in Ids)
                {
                    yield return id;
                }
            }
        }

        public HashSet<EnvironmentPurpose> Purposes { get; set; }
    }
}
