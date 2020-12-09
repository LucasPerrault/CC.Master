using System;

namespace Salesforce.Infra.Configurations
{
    public class SalesforceConfiguration
    {
        public Uri ServerUri { get; set; }
        public string AccountsEndpointPath { get; set; }
        public Guid Token { get; set; }
    }
}
