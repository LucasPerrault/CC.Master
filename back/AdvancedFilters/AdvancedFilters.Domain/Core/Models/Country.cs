using Lucca.Core.PublicData.Domain.Models;

namespace AdvancedFilters.Domain.Core.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static Country FromLuccaCountry(LuccaCountry luccaCountry)
        {
            return new Country { Id = luccaCountry.Id, Name = luccaCountry.Name };
        }
    }
}
