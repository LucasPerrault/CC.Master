using CloudControl.Shared.Infra.Proxy.Configuration;

namespace CloudControl.Shared.Infra.Configuration
{
	public class Configuration
	{
		public const string LuccaLoggerOptionsKey = "LuccaLoggerOptions";
		public const string AppName = "CloudControl";

		public LegacyCloudControlConfiguration LegacyCloudControl { get; set; }
	}
}
