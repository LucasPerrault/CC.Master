using System;
using System.Collections.Generic;
using Tools;

namespace Billing.Contracts.Domain.Contracts
{
    public class ContractFilter
    {
        public HashSet<int> Ids { get; set; } = new HashSet<int>();
        public Guid? ClientExternalId { get; set; }
        public HashSet<string> Search { get; set; } = new HashSet<string>();
        public CompareNullableDateTime ArchivedAt { get; set; } = CompareNullableDateTime.Bypass();
        public HashSet<int> EnvironmentIds { get; set; } = new HashSet<int>();
        public CompareBoolean HasEnvironment { get; set; } = CompareBoolean.Bypass;
        public HashSet<int> ClientIds { get; set; } = new HashSet<int>();
        public HashSet<int> DistributorIds { get; set; } = new HashSet<int>();
        public HashSet<int> ExcludedDistributorIds { get; set; } = new HashSet<int>();
        public HashSet<int> ProductIds { get; set; } = new HashSet<int>();
        public HashSet<int> CommercialOfferIds { get; set; } = new HashSet<int>();
        public CompareDateTime CreatedAt { get; set; } = CompareDateTime.Bypass();
        public CompareDateTime StartsOn { get; set; } = CompareDateTime.Bypass();
        public CompareNullableDateTime EndsOn { get; set; } = CompareNullableDateTime.Bypass();
        public HashSet<int> CurrentlyAttachedEstablishmentIds { get; set; } = new HashSet<int>();
        public HashSet<ContractStatus> ContractStatuses { get; set; } = new HashSet<ContractStatus>();
    }
}
