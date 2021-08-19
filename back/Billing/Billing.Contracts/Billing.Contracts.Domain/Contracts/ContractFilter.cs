using System;
using Tools;

namespace Billing.Contracts.Domain.Contracts
{
    public class ContractFilter
    {
        public Guid? ClientExternalId { get; set; }
        public CompareString Subdomain { get; set; }

        public static ContractFilter All => new ContractFilter
        {
            Subdomain = CompareString.Bypass
        };
    }
}
