using Authentication.Infra.Configurations;
using Core.Proxy.Infra.Configuration;
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
		public SalesforceConfiguration Salesforce { get; set; }
	}
}
