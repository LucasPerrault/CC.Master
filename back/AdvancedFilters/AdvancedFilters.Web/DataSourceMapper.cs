using AdvancedFilters.Domain.DataSources;
using AdvancedFilters.Web.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AdvancedFilters.Web
{
    public static class DataSourceMapper
    {
        public static Dictionary<DataSources, DataSource> GetAll(AdvancedFiltersConfiguration configuration)
        {
            return Enum.GetValues(typeof(DataSources)).Cast<DataSources>().ToDictionary(s => s, s => Get(s, configuration));
        }

        private static DataSource Get(DataSources source, AdvancedFiltersConfiguration configuration) => source switch
        {

            DataSources.Environments => configuration.Environment(),
            DataSources.Establishments => configuration.Establishment(),
            DataSources.AppInstances => configuration.AppInstance(),
            DataSources.LegalUnit => configuration.LegalUnit(),
            _ => throw new InvalidEnumArgumentException(nameof(source), (int)source, typeof(DataSources))
        };
    }
}
