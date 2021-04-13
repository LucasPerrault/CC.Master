using Instances.Domain.Demos;
using Instances.Domain.Instances;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Shared.Domain.Exceptions;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Instances.Application.Demos
{
    public class DemosRepository
    {
        private readonly ClaimsPrincipal _principal;
        private readonly IDemosStore _demosStore;
        private readonly IInstancesStore _instancesStore;
        private readonly IDemoRightsFilter _rightsFilter;

        public DemosRepository
        (
            ClaimsPrincipal principal,
            IDemosStore demosStore,
            IInstancesStore instancesStore,
            IDemoRightsFilter rightsFilters
        )
        {
            _principal = principal;
            _demosStore = demosStore;
            _instancesStore = instancesStore;
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

        public async Task<Demo> DeleteAsync(int id)
        {
            var demo = (
                await _demosStore.GetAsync(await _rightsFilter
                .GetDefaultReadFilterAsync(_principal), d => d.IsActive)
            ).SingleOrDefault(d => d.Id == id);

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
            return demo;
        }
    }
}
