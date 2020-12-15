using Remote.Infra.Configurations;
using System;

namespace Salesforce.Infra.Configurations
{
    public class SalesforceServiceConfiguration : RemoteServiceConfiguration
    {
        private const string _userAgent = "SalesforceService";
        private const string _authScheme = "Bearer";

        public SalesforceServiceConfiguration(Guid authToken)
            : base(authToken, _userAgent, _authScheme, string.Empty)
        { }

        protected override string AuthorizationHeaderParam(Guid token, string type) => token.ToString();
    }
}
