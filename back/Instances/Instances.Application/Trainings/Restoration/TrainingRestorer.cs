using Environments.Domain;
using Environments.Domain.Storage;
using Instances.Application.Instances;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Domain.Trainings;
using Lucca.Core.Shared.Domain.Exceptions;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System.Security.Claims;
using System.Threading.Tasks;
using CCEnvironment = Environments.Domain.Environment;

namespace Instances.Application.Trainings.Restoration
{
    public class TrainingRestorer
    {
        private readonly ClaimsPrincipal _principal;
        private readonly InstancesManipulator _instancesManipulator;
        private readonly ITrainingsStore _trainingsStore;
        private readonly ITrainingRestorationsStore _trainingRestorationsStore;
        private readonly IRightsService _rightsService;
        private readonly IEnvironmentsStore _environmentsStore;
        private readonly IInstancesStore _instancesStore;
        private readonly EnvironmentRightsFilter _environmentsRightsFilter;

        public TrainingRestorer
        (
            ClaimsPrincipal principal,
            InstancesManipulator instancesManipulator,
            ITrainingRestorationsStore trainingRestorationsStore,
            ITrainingsStore trainingsStore,
            IRightsService rightsService,
            IEnvironmentsStore environmentsStore,
            IInstancesStore instancesStore
        )
        {
            _principal = principal;
            _instancesManipulator = instancesManipulator;
            _trainingsStore = trainingsStore;
            _rightsService = rightsService;
            _environmentsStore = environmentsStore;
            _instancesStore = instancesStore;
            _trainingRestorationsStore = trainingRestorationsStore;
            _environmentsRightsFilter = new EnvironmentRightsFilter(_rightsService);
        }

        public async Task<TrainingRestoration> CreateRestorationAsync(TrainingRestorationRequest request)
        {
            await ThrowIfForbiddenAsync(request);

            var environment = await GetTrainingEnvironmentAsync(request.EnvironmentId);

            var previousInstance = _instancesStore.GetActiveInstanceFromEnvironmentId(request.EnvironmentId, InstanceType.Training);
            if (previousInstance != null)
            {
                await _instancesStore.DeleteByIdAsync(previousInstance.Id);
                await _trainingsStore.DeleteForInstanceAsync(previousInstance.Id);
            }

            var restoration = TrainingRestorationFactory.New
            (
                _principal,
                environment,
                request.Anonymize,
                request.RestoreFiles,
                request.KeepExistingTrainingPasswords,
                request.Comment,
                request.CommentExpiryDate
            );

            await _trainingRestorationsStore.CreateAsync(restoration);
            await _instancesManipulator.RequestRemoteBackupAsync(environment, InstanceType.Training);
            await _instancesManipulator.RequestRemoteDuplicationAsync
            (
                restoration.InstanceDuplication,
                InstanceDuplicationOptions.ForTraining(
                    withAnonymization: restoration.Anonymize,
                    keepExistingPasswords: restoration.KeepExistingTrainingPasswords,
                    callBackPath: $"/api/trainings/restorations/{restoration.InstanceDuplicationId}/notify"
                )
            );

            return restoration;
        }

        private async Task<CCEnvironment> GetTrainingEnvironmentAsync(int environmentId)
        {
            var accesses = await _environmentsRightsFilter.GetAccessRightAsync(_principal, Operation.ReadEnvironments);
            var trainingEnvironment = await _environmentsStore.GetActiveByIdAsync(accesses, environmentId);

            return trainingEnvironment
                ?? throw new BadRequestException($"Environment {environmentId} could not be found");
        }

        private async Task ThrowIfForbiddenAsync(TrainingRestorationRequest request)
        {
            await _rightsService.ThrowIfAnyOperationIsMissingAsync(Operation.RestoreInstances);

            var readEnvironmentsRights = await _environmentsRightsFilter.GetAccessRightAsync(_principal, Operation.ReadEnvironments);
            if (!await _environmentsStore.HasAccessAsync(readEnvironmentsRights, request.EnvironmentId))
            {
                throw new NotFoundException();
            }

            var restoreInstancesRights = await _environmentsRightsFilter.GetAccessRightAsync(_principal, Operation.RestoreInstances);
            if (!await _environmentsStore.HasAccessAsync(restoreInstancesRights, request.EnvironmentId))
            {
                throw new ForbiddenException("Insufficient rights to restore the training instance of this environment");
            }
        }
    }
}
