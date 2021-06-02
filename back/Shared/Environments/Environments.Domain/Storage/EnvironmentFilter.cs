using System.Collections.Generic;
using Tools;

namespace Environments.Domain.Storage
{
    public class EnvironmentFilter : ValueObject
    {
        public CompareString Subdomain { get; set; } = CompareString.Bypass;
        public CompareBoolean IsActive { get; set; } = CompareBoolean.Bypass;
        public string Search { get; set; }

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return Subdomain;
                yield return IsActive;
                yield return Search;
            }
        }
    }
}
