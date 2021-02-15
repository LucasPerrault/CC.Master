using System;

namespace Core.Proxy.Infra.Configuration
{
    public class LegacyCloudControlConfiguration
    {
        public string Host { get; set; }
        public Uri Uri => new UriBuilder { Host = Host, Scheme = "http" }.Uri;
    }
}
