using Environments.Domain;
using Instances.Domain.Trainings;
using Instances.Domain.Trainings.Filtering;
using Lucca.Core.Api.Abstractions.Paging;
using Rights.Domain;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Instances.Application.Trainings
{
    public class TrainingsRepository
    {
        private readonly ClaimsPrincipal _principal;
        private readonly ITrainingsStore _trainingsStore;
        private readonly EnvironmentRightsFilter _rightsFilter;

        public TrainingsRepository
        (
            ClaimsPrincipal principal,
            ITrainingsStore trainingsStore,
            EnvironmentRightsFilter rightsFilters
        )
        {
            _principal = principal;
            _trainingsStore = trainingsStore;
            _rightsFilter = rightsFilters;
        }

        public async Task<Page<Training>> GetTrainingsAsync(IPageToken pageToken, TrainingFilter filter)
        {
            var access = await _rightsFilter.GetAccessRightAsync(_principal, Operation.ReadEnvironments);
            return await _trainingsStore.GetAsync
            (
                pageToken,
                filter,
                access
            );
        }
    }
}
