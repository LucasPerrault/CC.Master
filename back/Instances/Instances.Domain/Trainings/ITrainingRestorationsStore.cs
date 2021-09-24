using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Instances.Domain.Trainings
{

    public interface ITrainingRestorationsStore
    {
        Task<TrainingRestoration> CreateAsync(TrainingRestoration restoration);
        Task<IReadOnlyCollection<TrainingRestoration>> GetByIdsAsync(IReadOnlyCollection<int> ids);
        Task<TrainingRestoration> GetByInstanceDuplicationIdAsync(Guid instanceDuplicationId);
    }
}
