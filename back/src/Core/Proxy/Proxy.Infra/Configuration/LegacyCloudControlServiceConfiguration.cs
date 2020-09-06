using Remote.Infra.Configurations;

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
