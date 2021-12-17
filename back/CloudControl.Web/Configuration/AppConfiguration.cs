using AdvancedFilters.Web.Configuration;
using Billing.Contracts.Infra.Configurations;
using Cache.Web;
using Core.Proxy.Infra.Configuration;
using Email.Infra.Configuration;
using Environments.Web;
using Instances.Web;
using Rights.Infra.Configuration;
using Salesforce.Infra.Configurations;
using System;
using TeamNotification.Web;
using Users.Web;
using AuthenticationConfiguration = Authentication.Infra.Configurations.AuthenticationConfiguration;

namespace CloudControl.Web.Configuration
{
    public class SlackConfiguration : ISlackConfiguration, Slack.Infra.ISlackConfiguration
    {
        public string Token { get; init; }

        public SlackHooks Hooks { get; init; }
    }

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
        public RedisConfiguration Redis { get; set; }
        public EmailConfiguration Email { get; set; }
        public SlackConfiguration Slack { get; set; }
        public EnvironmentConfiguration Environment { get; set; }

        public UsersConfiguration Users => new UsersConfiguration
        {
            ServerUri = Authentication.ServerUri,
            AllUsersEndpointPath = Authentication.AllUsersEndpointPath,
            UsersEndpointPath = Authentication.UsersEndpointPath,
            UserFetchToken =  Authentication.ApiKeysFetcherToken
        };

        public SqlConfiguration SqlInfos { get; set; }
        public AdvancedFiltersConfiguration AdvancedFilters { get; set; }
        public Uri Host { get; set; }
    }

    public class SqlConfiguration
    {
        public string Default { get; set; }
    }
}
