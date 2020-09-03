using Shared.Infra.Remote.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Proxy.Infra.Configuration
{
    public class LegacyCloudControlServiceConfiguration : RemoteServiceConfiguration
    {
        private const string _userAgent = "LegacyCloudControlService";

        public LegacyCloudControlServiceConfiguration()
            : base(_userAgent)
        { }
    }
}
