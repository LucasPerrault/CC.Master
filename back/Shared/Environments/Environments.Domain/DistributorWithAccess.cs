using System.Collections.Generic;
using System.Linq;
using Tools;

namespace Environments.Domain
{
    public class EnvironmentWithAccess : ValueObject
    {
        public int Id { get; set; }
        public string Subdomain { get; set; }
        public HashSet<DistributorWithAccess> Accesses { get; set; }

        protected override IEnumerable<object> EqualityComponents
        {
            get
            {
                yield return Id;
                yield return Subdomain;

                foreach (var access in Accesses)
                {
                    yield return access;
                }
            }
        }
    }
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
                return GetEqualityComponents();
            }
        }

        internal IEnumerable<object> GetEqualityComponents()
        {
            yield return DistributorCode;
            foreach (var type in AccessTypes)
            {
                yield return type;
            }
        }
    }
}
