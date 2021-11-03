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

        public static DataSource Get(DataSources source, AdvancedFiltersConfiguration configuration) => source switch
        {
            DataSources.Countries => configuration.Country(),
            DataSources.Environments => configuration.Environment(),
            DataSources.AppInstances => configuration.AppInstance(),
            DataSources.LegalUnits => configuration.LegalUnit(),
            DataSources.Establishments => configuration.Establishment(),
            DataSources.Clients => configuration.Client(),
            DataSources.Distributors => configuration.Distributor(),
            DataSources.EnvironmentAccesses => configuration.EnvironmentAccess(),
            DataSources.AppContacts => configuration.AppContact(),
            DataSources.ClientContacts => configuration.ClientContact(),
            DataSources.SpecializedContacts => configuration.SpecializedContact(),
            _ => throw new InvalidEnumArgumentException(nameof(source), (int)source, typeof(DataSources))
        };
    }
}
