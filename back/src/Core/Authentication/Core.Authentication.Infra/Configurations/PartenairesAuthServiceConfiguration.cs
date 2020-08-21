using CloudControl.Shared.Infra.Remote.Configurations;

namespace Core.Authentication.Infra.Configurations
{
    public class PartenairesAuthServiceConfiguration : RemoteServiceConfiguration
    {
        private const string _userAgent = "PartenairesService";

        public PartenairesAuthServiceConfiguration()
            : base(_userAgent)
        { }
    }
}
