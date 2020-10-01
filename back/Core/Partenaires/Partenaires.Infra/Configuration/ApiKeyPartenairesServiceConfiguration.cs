using Authentication.Domain;
using Remote.Infra.Configurations;

namespace Partenaires.Infra.Configuration
{
    public class ApiKeyPartenairesServiceConfiguration : RemoteServiceConfiguration
    {
        private const string _userAgent = "PartenairesService";
        private const string _authScheme = "Lucca";
        private const string _authType = "application";
        public ApiKeyPartenairesServiceConfiguration(ApiKey apiKey)
             : base(apiKey.Token, _userAgent, _authScheme, _authType)
        { }
    }
}
