using Shared.Infra.Remote.Configurations;

namespace Partenaires.Infra.Configuration
{
    public class PartenairesAuthServiceConfiguration : RemoteServiceConfiguration
    {
        private const string _userAgent = "PartenairesService";

        public PartenairesAuthServiceConfiguration()
            : base(_userAgent)
        { }
    }
}
