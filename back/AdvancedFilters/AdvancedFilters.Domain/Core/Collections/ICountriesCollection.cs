using AdvancedFilters.Domain.Core.Models;
using Lucca.Core.PublicData.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.Core.Collections
{
    public interface ICountriesCollection
    {
        Task<Country> GetByIdAsync(int id);
        Task<IReadOnlyCollection<Country>> GetAsync(IReadOnlyCollection<int> ids);
        Task<IReadOnlyCollection<Country>> GetAllAsync();
    }

    public class CountriesCollection : ICountriesCollection
    {
        private readonly ILuccaCountriesCollection _countriesCollection;

        public CountriesCollection(ILuccaCountriesCollection countriesCollection)
        {
            _countriesCollection = countriesCollection;
        }

        public Task<Country> GetByIdAsync(int id)
        {
            var country = GetCountry(id);
            return Task.FromResult(country);
        }

        public Task<IReadOnlyCollection<Country>> GetAsync(IReadOnlyCollection<int> ids)
        {
            var countries = ids
                .Select(GetCountry)
                .ToList();
            return Task.FromResult<IReadOnlyCollection<Country>>(countries);
        }

        public Task<IReadOnlyCollection<Country>> GetAllAsync()
        {
            IReadOnlyCollection<Country> countries = _countriesCollection
                .Countries
                .Select(Country.FromLuccaCountry)
                .ToList();

            return Task.FromResult(countries);
        }

        private Country GetCountry(int id)
        {
            var luccaCountry = _countriesCollection.GetById(id);
            return Country.FromLuccaCountry(luccaCountry);
        }
    }
}
