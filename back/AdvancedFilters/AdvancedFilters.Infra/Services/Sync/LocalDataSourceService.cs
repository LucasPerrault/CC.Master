using AdvancedFilters.Domain.Core.Collections;
using AdvancedFilters.Domain.Core.Models;
using AdvancedFilters.Domain.DataSources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Services.Sync
{
    public class LocalDataSourceService : ILocalDataSourceService
    {
        private readonly ICountriesCollection _countriesCollection;

        public LocalDataSourceService(ICountriesCollection countriesCollection)
        {
            _countriesCollection = countriesCollection;
        }

        public async Task<IReadOnlyCollection<Country>> GetAllCountriesAsync()
        {
            return await _countriesCollection.GetAllAsync();
        }
    }
}
