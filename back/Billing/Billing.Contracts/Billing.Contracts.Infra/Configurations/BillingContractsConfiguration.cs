using System;

namespace Billing.Contracts.Infra.Configurations
{
    public class BillingContractsConfiguration
    {
        public string LegacyClientsEndpointPath { get; set; }
        public Guid TenantCountsApiWebServiceToken { get; set; }
    }
}
