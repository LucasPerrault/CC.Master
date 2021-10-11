using System.Collections.Generic;
using Tools;

namespace Environments.Domain.Storage
{
    public class EnvironmentFilter : ValueObject
    {
        public CompareString Subdomain { get; set; } = CompareString.Bypass;
        public CompareBoolean IsActive { get; set; } = CompareBoolean.Bypass;
        public string Search { get; set; }

        public HashSet<int> Ids { get; set; } = new HashSet<int>();
        public HashSet<string> Clusters { get; set; } = new HashSet<string>();
        public HashSet<EnvironmentPurpose> Purposes { get; set; }
        public HashSet<EnvironmentDomain> Domains { get; set; }

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

                foreach (var cluster in Clusters)
                {
                    yield return cluster;
                }

                foreach (var purpose in Purposes)
                {
                    yield return purpose;
                }

                foreach (var domain in Domains)
                {
                    yield return domain;
                }
            }
        }
    }
}
