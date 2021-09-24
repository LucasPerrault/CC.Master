using Environments.Domain.Storage;
using Instances.Application.Instances;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Domain.Shared;
using Instances.Domain.Trainings;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Instances.Application.Trainings.Restoration
{
    public interface ITrainingRestorationCompleter
    {
        Task MarkRestorationAsCompletedAsync(Guid instanceDuplicationId, bool isSuccessful);
    }

    public class TrainingRestorationCompleter : ITrainingRestorationCompleter
    {
        private readonly ITrainingsStore _trainingsStore;
        private readonly IInstancesStore _instancesStore;
        private readonly ITrainingRestorationsStore _restorarationsStore;
        private readonly IInstanceDuplicationsStore _instanceDuplicationsStore;
        private readonly TrainingRestorationLogsRepository _trainingRestorationLogsRepository;
        private readonly IWsAuthSynchronizer _wsAuthSynchronizer;
        private readonly InstancesManipulator _instancesManipulator;
        private readonly ILogger<TrainingRestorationCompleter> _logger;

        public TrainingRestorationCompleter
        (
            ITrainingRestorationsStore trainingRestorationsStore,
            IInstanceDuplicationsStore instanceDuplicationsStore,
            ITrainingsStore trainingsStore,
            IInstancesStore instancesStore,
            TrainingRestorationLogsRepository trainingRestorationLogsRepository,
            IWsAuthSynchronizer wsAuthSynchronizer,
            InstancesManipulator instancesManipulator,
            ILogger<TrainingRestorationCompleter> logger
        )
        {
            _restorarationsStore = trainingRestorationsStore;
            _instanceDuplicationsStore = instanceDuplicationsStore;
            _trainingsStore = trainingsStore;
            _instancesStore = instancesStore;
            _trainingRestorationLogsRepository = trainingRestorationLogsRepository;
            _wsAuthSynchronizer = wsAuthSynchronizer;
            _instancesManipulator = instancesManipulator;
            _logger = logger;
        }

        public async Task MarkRestorationAsCompletedAsync(Guid instanceDuplicationId, bool isSuccessful)
        {
            var restoration = await _restorarationsStore.GetByInstanceDuplicationIdAsync(instanceDuplicationId);
            if(restoration == null)
            {
                throw new BadRequestException($"Unknown duplication {instanceDuplicationId}");
            }
            if (restoration.HasEnded)
            {
                throw new BadRequestException($"Duplication {instanceDuplicationId} was already marked as complete");
            }

            if (!isSuccessful)
            {
                await _instanceDuplicationsStore.MarkAsCompleteAsync(restoration.InstanceDuplication, InstanceDuplicationProgress.FinishedWithFailure);
                await _trainingRestorationLogsRepository.LogFailedTrainingRestorationAsync(restoration);
                return;
            }

            try
            {
                var instance = await _instancesStore.CreateForTrainingAsync(restoration.EnvironmentId, restoration.Anonymize);
                await CreateAndStoreTrainingAsync(restoration, instance);
                await _instancesManipulator.RequestResetInstanceCacheAsync(restoration.Environment, InstanceType.Training);
                await _instanceDuplicationsStore.MarkAsCompleteAsync(restoration.InstanceDuplication, InstanceDuplicationProgress.FinishedWithSuccess);
                await _trainingRestorationLogsRepository.LogSuccessfulTrainingRestorationAsync(restoration);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Training restoration failed (instanceDuplication : {id})", instanceDuplicationId);
                await _instanceDuplicationsStore.MarkAsCompleteAsync(restoration.InstanceDuplication, InstanceDuplicationProgress.FinishedWithFailure);
                await _trainingRestorationLogsRepository.LogFailedTrainingRestorationAsync(restoration);
            }
        }

        private async Task CreateAndStoreTrainingAsync(TrainingRestoration restoration, Instance instance)
        {
            var training = BuildTraining(restoration, instance);
            try
            {
                await _trainingsStore.CreateAsync(training);
                await _wsAuthSynchronizer.SafeSynchronizeAsync(instance.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Could not create training, following instance duplication, (restoration '{restoration.Id}')");
                await _instancesStore.DeleteByIdAsync(instance.Id);
                await _trainingsStore.DeleteAsync(training);
                throw;
            }
        }

        private Training BuildTraining(TrainingRestoration restoration, Instance instance)
        {
            var training = new Training
            {
                LastRestoredAt = DateTime.Now,
                AuthorId = restoration.AuthorId,
                ApiKeyStorableId = restoration.ApiKeyStorableId,
                IsActive = true,
                InstanceId = instance.Id,
                EnvironmentId = restoration.EnvironmentId,
                TrainingRestorationId = restoration.Id,
            };

            return training;
        }
    }
}
