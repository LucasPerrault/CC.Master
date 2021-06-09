using System.Collections.Generic;
using System.Linq;
using Tools;

namespace Environments.Domain
{
    public class DistributorWithAccess : ValueObject
    {
        public string DistributorCode { get; }
        public List<EnvironmentAccessTypeEnum> AccessTypes { get; }

        public DistributorWithAccess(string distributorCode, IEnumerable<EnvironmentAccessTypeEnum> types)
        {
            DistributorCode = distributorCode;
            AccessTypes = types.ToList();
        }

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return DistributorCode;
                foreach (var type in AccessTypes)
                {
                    yield return type;
                }
            }
        }
    }
}
