using System;
using Tools;

namespace Billing.Contracts.Domain.Contracts
{
    public class ContractFilter
    {
        public Guid? ClientExternalId { get; set; }
        public CompareString Subdomain { get; set; } = CompareString.Bypass;
        public CompareNullableDateTime ArchivedAt { get; set; } = CompareNullableDateTime.Bypass();

        public static ContractFilter AllNotArchived() => new ContractFilter
        {
            Subdomain = CompareString.Bypass,
            ArchivedAt = CompareDateTime.IsAfterOrEqual(DateTime.Now).OrNull()
        };
    }
}
