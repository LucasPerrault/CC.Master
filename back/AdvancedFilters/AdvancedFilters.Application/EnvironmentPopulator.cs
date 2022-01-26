using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedFilters.Domain.Facets;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Application;

public interface IEnvironmentPopulator
{
    Task PopulateAsync(IEnumerable<Environment> environments, HashSet<FacetIdentifier> facetIdentifiers);
}

public class EnvironmentPopulator : IEnvironmentPopulator
{
    private readonly IFacetsStore _facetsStore;

    public EnvironmentPopulator(IFacetsStore facetsStore)
    {
        _facetsStore = facetsStore;
    }

    public async Task PopulateAsync(IEnumerable<Environment> environments, HashSet<FacetIdentifier> facetIdentifiers)
    {
        if (!facetIdentifiers.Any())
        {
            return;
        }

        var environmentIds = environments.Select(e => e.Id).ToHashSet();

        var filter = EnvironmentFacetValueFilter.ForEnvironments(environmentIds, facetIdentifiers);
        var facetValues = await _facetsStore.GetValuesAsync(filter);

        var group = facetValues
            .GroupBy(f => f.EnvironmentId)
            .ToDictionary(g => g.Key, g => g);

        foreach (var environment in environments.Where(e => group.ContainsKey(e.Id)))
        {
            environment.Facets = group[environment.Id];
        }
    }
}
