using System;

namespace AdvancedFilters.Web.Configuration
{
    public class AuthenticationConfiguration
    {
        public string CloudControlAuthScheme { get; set; }
        public string CloudControlAuthParameter { get; set; }
        public Guid LuccaWebserviceToken { get; set; }
        public Guid ClientCenterWebserviceToken { get; set; }
    }
}
