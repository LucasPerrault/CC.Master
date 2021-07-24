using AdvancedFilters.Domain;
using AdvancedFilters.Web.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedFilters.Web
{

    public class AdvancedFiltersConfiguration
    {
        public RoutesConfiguration Routes { get; set; }
    }

    public static class AdvancedFiltersConfigurer
    {
        public static void ConfigureServices(IServiceCollection services, AdvancedFiltersConfiguration configuration)
        {
            services.AddSingleton(new DataSourceRepository(DataSourceMapper.GetAll(configuration)));
        }
    }

    public static class DataSourceMapper
    {
        public static Dictionary<DataSources, DataSource> GetAll(AdvancedFiltersConfiguration configuration)
        {
            var routes = DataSourceRouteMapper.ToRoutes(configuration.Routes);

            return Enum.GetValues(typeof(DataSources)).Cast<DataSources>()
                .ToDictionary(s => s, s => new DataSource
                {
                    Source = s,
                    Route = routes[s]
                });
        }
    }
}
