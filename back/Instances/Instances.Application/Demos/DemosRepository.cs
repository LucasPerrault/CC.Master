using Instances.Domain.Demos;
using Instances.Domain.Demos.Filtering;
using Instances.Domain.Instances;
using Instances.Domain.Shared;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Shared.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Tools;

namespace Instances.Application.Demos
{
    public class DemosRepository
    {
        private readonly ClaimsPrincipal _principal;
        private readonly IDemosStore _demosStore;
        private readonly IInstancesStore _instancesStore;
        private readonly IDemoRightsFilter _rightsFilter;
        private readonly ICcDataService _ccDataService;

        public DemosRepository
        (
            ClaimsPrincipal principal,
            IDemosStore demosStore,
            IInstancesStore instancesStore,
            IDemoRightsFilter rightsFilters,
            ICcDataService ccDataService
        )
        {
            _principal = principal;
            _demosStore = demosStore;
            _instancesStore = instancesStore;
            _rightsFilter = rightsFilters;
            _ccDataService = ccDataService;
        }

        public async Task<Page<Demo>> GetDemosAsync(DemoListQuery query)
        {
            var access = await _rightsFilter.GetReadAccessAsync(_principal);
            return await _demosStore.GetAsync(
                query.Page,
                query.ToDemoFilter(access)
            );
        }

        public async Task<Demo> DeleteAsync(int id)
        {
            var activeDemosInScope = await _demosStore.GetAsync
            (
                new DemoFilter(await _rightsFilter.GetReadAccessAsync(_principal))
                {
                    IsActive = BoolCombination.TrueOnly
                }
            );

            var demo = activeDemosInScope.SingleOrDefault(d => d.Id == id);

            if (demo == null)
            {
                throw new NotFoundException();
            }

            if (demo.IsTemplate)
            {
                throw new ForbiddenException("Template demos cannot be deleted");
            }

            if (demo.Instance.IsProtected)
            {
                throw new BadRequestException($"Demo {demo.Id} is protected and cannot be deleted");
            }

            await _instancesStore.DeleteForDemoAsync(demo.Instance);
            await _demosStore.DeleteAsync(demo);
            await _ccDataService.DeleteInstanceAsync(demo.Subdomain, demo.Instance.Cluster, String.Empty);
            return demo;
        }
    }
}
