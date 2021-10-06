using AdvancedFilters.Domain.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.DataSources
{
    public interface ILocalDataSourceService
    {
        Task<List<Country>> GetAllCountriesAsync();
    }
}
