using System;
using System.Collections.Generic;
using Tools;

namespace Billing.Contracts.Domain.Contracts
{
    public class ContractFilter
    {
        public int? Id { get; set; }
        public Guid? ClientExternalId { get; set; }
        public HashSet<string> Search { get; set; } = new HashSet<string>();
        public HashSet<string> Subdomains { get; set; } = new HashSet<string>();
        public CompareNullableDateTime ArchivedAt { get; set; } = CompareNullableDateTime.Bypass();
        public CompareDateTime StartsOn { get; set; } = CompareDateTime.Bypass();
        public CompareNullableDateTime EndsOn { get; set; } = CompareNullableDateTime.Bypass();
    }
}
