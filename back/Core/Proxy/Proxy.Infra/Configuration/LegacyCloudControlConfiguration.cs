using Remote.Infra.Extensions;
using System;

namespace Core.Proxy.Infra.Configuration
{
	public class LegacyCloudControlConfiguration
	{
		public string Host { get; set; }
		public Uri LegacyEndpoint(string endpoint = null)
			=> new UriBuilder { Host = Host, Scheme = "http", Path = endpoint.AsSafeEndpoint() }.Uri;
	}
}
