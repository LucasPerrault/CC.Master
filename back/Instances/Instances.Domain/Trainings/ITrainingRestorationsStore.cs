using Environments.Domain.Storage;
using Instances.Domain.Trainings.Filtering;
using Lucca.Core.Api.Abstractions.Paging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.Trainings
{

    public interface ITrainingRestorationsStore
    {
        Task<TrainingRestoration> CreateAsync(TrainingRestoration restoration);
        Task<Page<TrainingRestoration>> GetAsync(IPageToken pageToken, TrainingRestorationFilter filter, List<EnvironmentAccessRight> access);
        Task<IReadOnlyCollection<TrainingRestoration>> GetByIdsAsync(IReadOnlyCollection<int> ids);
        Task<TrainingRestoration> GetByInstanceDuplicationIdAsync(Guid instanceDuplicationId);
        Task<TrainingRestoration> GetActiveByEnvironmentIdAsync (int environmentId);
    }
}
