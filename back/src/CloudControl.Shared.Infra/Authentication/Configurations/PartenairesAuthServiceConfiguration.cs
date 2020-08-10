using CloudControl.Shared.Infra.Remote.Configurations;

namespace CloudControl.Shared.Infra.Authentication.Configurations
{
    public class PartenairesAuthServiceConfiguration : RemoteServiceConfiguration
    {
        private const string _userAgent = "PartenairesService";

        public PartenairesAuthServiceConfiguration()
            : base(_userAgent)
        { }
    }
}
