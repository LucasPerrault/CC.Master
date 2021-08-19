using System;

namespace Billing.Contracts.Domain.Clients
{
    public class ClientFilter
    {
        public Guid? ExternalId { get; set; }
        public static ClientFilter All => new ClientFilter();
    }
}
