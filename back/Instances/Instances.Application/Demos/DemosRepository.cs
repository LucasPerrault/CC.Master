using System.Linq;
using Instances.Application.Demos.Dtos;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Filtering;
using Instances.Domain.Instances;
using Instances.Domain.Shared;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Shared.Domain.Exceptions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Instances.Application.Demos
{
    public class DemosRepository
    {
        private readonly ClaimsPrincipal _principal;
        private readonly IDemosStore _demosStore;
        private readonly IInstancesStore _instancesStore;
        private readonly DemoRightsFilter _rightsFilter;
        private readonly ICcDataService _ccDataService;
        private readonly IDnsService _dnsService;

        public DemosRepository
        (
            ClaimsPrincipal principal,
            IDemosStore demosStore,
            IInstancesStore instancesStore,
            DemoRightsFilter rightsFilters,
            ICcDataService ccDataService,
            IDnsService dnsService
        )
        {
            _principal = principal;
            _demosStore = demosStore;
            _instancesStore = instancesStore;
            _rightsFilter = rightsFilters;
            _ccDataService = ccDataService;
            _dnsService = dnsService;
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

        public async Task<Demo> GetByIdAsync(int id)
        {
            return await GetSingleOrDefaultAsync(DemoFilter.ById(id));
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

            await _dnsService.DeleteAsync(DnsEntry.ForDemo(demo.Subdomain, demo.Cluster));
            await _instancesStore.DeleteByIdAsync(demo.InstanceID);
            await _demosStore.DeleteAsync(demo);
            await _ccDataService.DeleteInstanceAsync(demo.Subdomain, demo.Cluster, string.Empty);
            return demo;
        }

        private async Task<Demo> GetSingleOrDefaultAsync(DemoFilter filter)
        {
            var access = await _rightsFilter.GetReadAccessAsync(_principal);
            var demos = await _demosStore.GetAsync(filter, access);
            var demo = demos.SingleOrDefault();
            if (demo == null)
            {
                throw new NotFoundException("Unknown demo");
            }

            return demo;
        }

        private async Task<Demo> GetSingleActiveDemoAsync(int id)
        {
            var access = await _rightsFilter.GetReadAccessAsync(_principal);

            var demo = await _demosStore.GetActiveByIdAsync(id, access);
            if (demo == null)
            {
                throw new NotFoundException();
            }

            return demo;
        }
    }
}
