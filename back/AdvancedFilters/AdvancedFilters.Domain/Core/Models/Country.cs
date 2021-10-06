using Lucca.Core.PublicData.Domain.Models;

namespace AdvancedFilters.Domain.Core.Models
{
    public class Country
    {
        public int Id { get; }
        public string Name { get; }

        public Country(LuccaCountry luccaCountry)
        {
            Id = luccaCountry.Id;
            Name = luccaCountry.Name;
        }
    }
}
