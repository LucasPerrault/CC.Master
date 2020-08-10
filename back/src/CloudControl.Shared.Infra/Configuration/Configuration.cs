using CloudControl.Shared.Infra.Proxy.Configuration;

namespace CloudControl.Shared.Infra.Configuration
{
	public class Configuration
	{
		public const string LuccaLoggerOptionsKey = "LuccaLoggerOptions";
		public const string AppName = "CloudControl";

		public AuthenticationConfiguration Authentication { get; set; }
		public ApiKeysConfiguration ApiKeys { get; set; }
		public LegacyCloudControlConfiguration LegacyCloudControl { get; set; }
	}
}
