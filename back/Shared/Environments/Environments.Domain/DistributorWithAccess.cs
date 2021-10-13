using Distributors.Domain.Models;
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

    public class AccessDistributor
    {
        public string Code { get; set; }
        public bool IsAllowingCommercialCommunication { get; set; }
    }

    public class DistributorWithAccess : ValueObject
    {
        public AccessDistributor Distributor { get; set; }
        public List<EnvironmentAccessTypeEnum> AccessTypes { get; }

        public DistributorWithAccess(Distributor distributor, IEnumerable<EnvironmentAccessTypeEnum> types)
        {
            Distributor = new AccessDistributor
            {
                Code = distributor.Code,
                IsAllowingCommercialCommunication = distributor.IsAllowingCommercialCommunication
            };
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
            yield return Distributor.Code;
            yield return Distributor.IsAllowingCommercialCommunication;
            foreach (var type in AccessTypes)
            {
                yield return type;
            }
        }
    }
}
