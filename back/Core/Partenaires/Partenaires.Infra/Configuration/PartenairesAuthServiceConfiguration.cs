using Remote.Infra.Configurations;

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
