using AdvancedFilters.Domain.Facets;
using Lucca.Core.Api.Abstractions.Paging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Storage.Stores
{
    public class FacetsStore : IFacetsStore
    {
        private readonly AdvancedFiltersDbContext _dbContext;

        public FacetsStore(AdvancedFiltersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Page<IEnvironmentFacetValue>> GetAsync(IPageToken pageToken, EnvironmentFacetFilter filter)
        {
            throw new NotImplementedException();
        }

        public Task<List<IEnvironmentFacetValue>> GetAsync(EnvironmentFacetFilter filter)
        {
            throw new NotImplementedException();
        }
    }
}
