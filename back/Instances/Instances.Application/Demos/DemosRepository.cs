using Instances.Domain.Demos;
using Lucca.Core.Api.Abstractions.Paging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Instances.Application.Demos
{
    public class DemosRepository
    {
        private readonly ClaimsPrincipal _principal;
        private readonly IDemosStore _demosStore;
        private readonly IDemoRightsFilter _rightsFilter;

        public DemosRepository(ClaimsPrincipal principal, IDemosStore demosStore, IDemoRightsFilter rightsFilters)
        {
            _principal = principal;
            _demosStore = demosStore;
            _rightsFilter = rightsFilters;
        }

        public async Task<Page<Demo>> GetDemosAsync(DemoListQuery query)
        {
            return await _demosStore.GetAsync(
                query.Page,
                d => d.IsActive == query.IsActive,
                await _rightsFilter.GetDefaultReadFilterAsync(_principal)
            );
        }
    }
}
