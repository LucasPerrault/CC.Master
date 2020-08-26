using Authentication.Infra.Configurations;
using Core.Proxy.Infra.Configuration;

namespace CloudControl.Web.Configuration
{
	public class AppConfiguration
	{
		public const string LuccaLoggerOptionsKey = "LuccaLoggerOptions";
		public const string AppName = "CloudControl";

		public AuthenticationConfiguration Authentication { get; set; }
		public ApiKeysConfiguration ApiKeys { get; set; }
		public LegacyCloudControlConfiguration LegacyCloudControl { get; set; }
	}
}
