using System;
using System.Collections.Generic;
using Tools;

namespace Billing.Contracts.Domain.Contracts
{
    public class ContractFilter
    {
        public Guid? ClientExternalId { get; set; }
        public HashSet<string> Search { get; set; } = new HashSet<string>();
        public CompareString Subdomain { get; set; } = CompareString.Bypass;
        public CompareNullableDateTime ArchivedAt { get; set; } = CompareNullableDateTime.Bypass();

        public static ContractFilter AllNotArchived() => new ContractFilter
        {
            Subdomain = CompareString.Bypass,
            ArchivedAt = CompareDateTime.IsAfterOrEqual(DateTime.Now).OrNull()
        };
    }
}
