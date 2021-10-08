using AdvancedFilters.Domain.Core.Models;
using AdvancedFilters.Domain.Instance.Models;
using System.Collections.Generic;

namespace AdvancedFilters.Infra.Services.Sync
{
    public static class CountryFixup
    {
        public static readonly Country DefaultCountry = new Country { Id = 0, Name = "Inconnu" };

        public static void FixCountryIfNeeded(this LegalUnit legalUnit, HashSet<int> knownCountryIds)
        {
            if (!knownCountryIds.Contains(legalUnit.CountryId))
            {
                legalUnit.CountryId = DefaultCountry.Id;
            }
        }
    }
}
