using System.Collections.Generic;

namespace AdvancedFilters.Domain.Billing.Filters
{
    public class LegalUnitFilter
    {
        public IReadOnlyCollection<int> CountryIds { get; set; }
        public IReadOnlyCollection<int> EnvironmentIds { get; set; }
    }
}
