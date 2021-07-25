using AdvancedFilters.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedFilters.Web.Configuration
{
    public class AuthenticationConfiguration
    {
        public string CloudControlAuthScheme { get; set; }
        public string CloudControlAuthParameter { get; set; }
        public Guid LuccaWebserviceToken { get; set; }
    }

    public static class DataSourceAuthMapper
    {
        private static readonly IReadOnlyDictionary<DataSources, Func<AuthenticationConfiguration, IDataSourceAuthentication>> All = new Dictionary<DataSources, Func<AuthenticationConfiguration, IDataSourceAuthentication>>
        {
            [DataSources.Environments] = c => new AuthorizationAuthentication(c.CloudControlAuthScheme, c.CloudControlAuthParameter),
            [DataSources.Establishments] = c => new LuccaAuthentication(c.LuccaWebserviceToken)
        };

        public static IReadOnlyDictionary<DataSources, IDataSourceAuthentication> ToAuths(AuthenticationConfiguration configuration)
        {
            return All.ToDictionary(e => e.Key, e => e.Value(configuration));
        }
    }
}
