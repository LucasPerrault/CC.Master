using AdvancedFilters.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedFilters.Web.Configuration
{
    public class RoutesConfiguration
    {

        public HostRoutesConfiguration Hosts { get; set; }
        public TenantRoutesConfiguration Tenants { get; set; }
    }

    public class TenantRoutesConfiguration
    {
        public string EstablishmentsEndpoint { get; set; }
    }

    public class HostRoutesConfiguration
    {
        public string CloudControlHost { get; set; }
    }

    public static class DataSourceRouteMapper
    {
        private static readonly IReadOnlyDictionary<DataSources, Func<RoutesConfiguration, IDataSourceRoute>> Mappings
            = new Dictionary<DataSources, Func<RoutesConfiguration, IDataSourceRoute>>
        {
            [DataSources.Environments] =  c => new HostDataSourceRoute
            {
                Host =  c.Hosts.CloudControlHost
            },
            [DataSources.Establishments] = c => new TenantDataSourceRoute
            {
                Endpoint = c.Tenants.EstablishmentsEndpoint
            }
        };

        public static Dictionary<DataSources, IDataSourceRoute> ToRoutes(RoutesConfiguration configuration)
        {
            return Mappings.ToDictionary(entry => entry.Key, entry => entry.Value(configuration));
        }
    }
}
