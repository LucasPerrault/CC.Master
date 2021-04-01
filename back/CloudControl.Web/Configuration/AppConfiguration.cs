using Authentication.Infra.Configurations;
using Billing.Contracts.Infra.Configurations;
using Core.Proxy.Infra.Configuration;
using Instances.Web;
using Rights.Infra.Configuration;
using Salesforce.Infra.Configurations;

namespace CloudControl.Web.Configuration
{
    public class AppConfiguration
    {
        public const string LuccaLoggerOptionsKey = "LuccaLoggerOptions";
        public const string AppName = "CloudControl";

        public AuthenticationConfiguration Authentication { get; set; }
        public RightsConfiguration Rights { get; set; }
        public LegacyCloudControlConfiguration LegacyCloudControl { get; set; }
        public BillingContractsConfiguration BillingContracts { get; set; }
        public SalesforceConfiguration Salesforce { get; set; }
        public InstancesConfigurer.InstancesConfiguration Instances { get; set; }
    }
}
