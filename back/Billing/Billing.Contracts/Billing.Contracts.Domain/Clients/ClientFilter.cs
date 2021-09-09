using System;
using System.Collections.Generic;

namespace Billing.Contracts.Domain.Clients
{
    public class ClientFilter
    {
        public Guid? ExternalId { get; set; }
        public HashSet<string> Search { get; set; }
        public static ClientFilter All => new ClientFilter();
    }
}
