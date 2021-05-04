using Instances.Application.Demos.Dtos;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Filtering;
using Instances.Domain.Instances;
using Instances.Domain.Shared;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Shared.Domain.Exceptions;
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
        private readonly DemoRightsFilter _rightsFilter;
        private readonly ICcDataService _ccDataService;

        public DemosRepository
        (
            ClaimsPrincipal principal,
            IDemosStore demosStore,
            IInstancesStore instancesStore,
            DemoRightsFilter rightsFilters,
            ICcDataService ccDataService
        )
        {
            _principal = principal;
            _demosStore = demosStore;
            _instancesStore = instancesStore;
            _rightsFilter = rightsFilters;
            _ccDataService = ccDataService;
        }

        public async Task<Page<Demo>> GetDemosAsync(IPageToken pageToken, DemoFilter filter)
        {
            var access = await _rightsFilter.GetReadAccessAsync(_principal);
            return await _demosStore.GetAsync
            (
                pageToken,
                filter,
                access
            );
        }

        public async Task<Demo> UpdateDemoAsync(int id, DemoPutPayload payload)
        {
            var demo = await GetSingleActiveDemoAsync(id);

            if (demo.IsTemplate)
            {
                throw new ForbiddenException("Template demos cannot be updated");
            }

            if (payload.Comment == null)
            {
                throw new BadRequestException("Missing payload field 'Comment'");
            }

            await _demosStore.UpdateCommentAsync(demo, payload.Comment);
            return demo;
        }

        public async Task<Demo> DeleteAsync(int id)
        {
            var demo = await GetSingleActiveDemoAsync(id);

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
            await _ccDataService.DeleteInstanceAsync(demo.Subdomain, demo.Instance.Cluster, string.Empty);
            return demo;
        }

        private async Task<Demo> GetSingleActiveDemoAsync(int id)
        {
            var access = await _rightsFilter.GetReadAccessAsync(_principal);
            var activeDemosInScope = await _demosStore.GetAsync
            (
                new DemoFilter { IsActive = BoolCombination.TrueOnly },
                access
            );

            var demo = activeDemosInScope.SingleOrDefault(d => d.Id == id);

            if (demo == null)
            {
                throw new NotFoundException();
            }

            return demo;
        }
    }
}
