using Environments.Domain.Storage;
using Instances.Domain.Trainings.Filtering;
using Lucca.Core.Api.Abstractions.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.Trainings
{
    public interface ITrainingsStore
    {
        Task<List<Training>> GetAsync(TrainingFilter filter, List<EnvironmentAccessRight> rights);
        Task<Page<Training>> GetAsync(IPageToken pageToken, TrainingFilter filter, List<EnvironmentAccessRight> rights);
        Task<Training> GetActiveByIdAsync(int id, List<EnvironmentAccessRight> rights);
        Task<Training> CreateAsync(Training training);
        Task DeleteAsync(Training training);
        Task DeleteForInstanceAsync(int instanceId);
    }
}
