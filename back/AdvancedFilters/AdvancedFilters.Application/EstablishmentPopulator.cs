using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedFilters.Domain.Facets;
using AdvancedFilters.Domain.Instance.Models;

namespace AdvancedFilters.Application;

public interface IEstablishmentPopulator
{
    Task PopulateAsync(IEnumerable<Establishment> establishments, HashSet<int> facetIds);
}

public class EstablishmentPopulator : IEstablishmentPopulator
{
    private readonly IFacetsStore _facetsStore;

    public EstablishmentPopulator(IFacetsStore facetsStore)
    {
        _facetsStore = facetsStore;
    }

    public async Task PopulateAsync(IEnumerable<Establishment> establishments, HashSet<int> facetIds)
    {
        if (!facetIds.Any())
        {
            return;
        }

        var establishmentIds = establishments.Select(e => e.Id).ToHashSet();

        var filter = EstablishmentFacetValueFilter.ForEstablishments(establishmentIds, facetIds);
        var facetValues = await _facetsStore.GetValuesAsync(filter);

        var group = facetValues
            .GroupBy(f => f.EstablishmentId)
            .ToDictionary(g => g.Key, g => g);

        foreach (var establishment in establishments.Where(e => group.ContainsKey(e.Id)))
        {
            establishment.Facets = group[establishment.Id];
        }
    }
}
