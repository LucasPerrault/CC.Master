using Environments.Infra.Storage;
using Environments.Infra.Storage.Stores;
using Instances.Application.CodeSources;
using Instances.Application.Instances;
using Instances.Application.Specflow.Tests.Shared.Tooling;
using Instances.Application.Specflow.Tests.Trainings.Models;
using Instances.Application.Trainings.Restoration;
using Instances.Domain.CodeSources;
using Instances.Domain.Github;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Domain.Shared;
using Instances.Domain.Trainings;
using Instances.Infra.DataDuplication;
using Instances.Infra.Instances.Services;
using Instances.Infra.Storage;
using Instances.Infra.Storage.Stores;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Testing.Specflow;
using Xunit;
using Environment = Environments.Domain.Environment;

namespace Instances.Application.Specflow.Tests.Trainings.Steps
{
    [Binding]
    public class TrainingRestorationStep
    {
        private readonly SpecflowTestContext _testContext;
        private readonly InstancesDbContext _instancesDbContext;
        private readonly EnvironmentsDbContext _environmentsDbContext;
        private readonly TrainingRestorationTestResults _results;


        public TrainingRestorationStep(SpecflowTestContext testContext, InstancesDbContext instancesDbContext, EnvironmentsDbContext environmentsDbContext, TrainingRestorationTestResults results)
        {
            _testContext = testContext;
            _instancesDbContext = instancesDbContext;
            _environmentsDbContext = environmentsDbContext;
            _results = results;
        }

        [When("I request the training restoration of environment '(.*)'")]
        public async Task WhenIRequestTheTrainingRestorationOfEnvironment(string subdomain)
        {
            var env = _instancesDbContext.Set<Environment>().Single(e => e.Subdomain == subdomain);
            _results.EnvironmentToRestore = env;

            var previousInstance = _instancesDbContext.Set<Instance>().SingleOrDefault(e => e.EnvironmentId == env.Id && e.IsActive);
            _results.PreviousTrainingInstanceId = previousInstance?.Id ?? null;

            var restorer = GetRestorer();
            var restorationRequest = new TrainingRestorationRequest
            {
                Anonymize = true,
                Comment = "hello",
                CommentExpiryDate = DateTime.Now,
                EnvironmentId = env.Id,
                KeepExistingTrainingPasswords = true,
                RestoreFiles = false,
            };

            try
            {
                _results.TrainingRestorationCreated = await restorer.CreateRestorationAsync(restorationRequest);
            }
            catch (Exception e)
            {
                _testContext.ThrownException = e;
            }
        }

        [When("I request the training restoration of environment '(.*)' (.*) anonymisation, (.*) keeping existing passwords, (.*) restoring files and the comment is '(.*)' with the expiry date (.*)")]
        public async Task WhenIRequestTheTrainingRestorationOfEnvironmentWithOptions(string subdomain, bool anonymize, bool keepExistingTrainingPasswords, bool restoreFiles, string comment, DateTime? expiryDate)
        {
            var env = _instancesDbContext.Set<Environment>().Single(e => e.Subdomain == subdomain);
            _results.EnvironmentToRestore = env;

            var previousInstance = _instancesDbContext.Set<Instance>().SingleOrDefault(e => e.EnvironmentId == env.Id && e.IsActive);
            _results.PreviousTrainingInstanceId = previousInstance?.Id ?? null;

            var restorer = GetRestorer();
            var restorationRequest = new TrainingRestorationRequest
            {
                Anonymize = anonymize,
                Comment = comment,
                CommentExpiryDate = expiryDate,
                EnvironmentId = env.Id,
                KeepExistingTrainingPasswords = keepExistingTrainingPasswords,
                RestoreFiles = restoreFiles,
            };

            _results.TrainingRestorationRequest = restorationRequest;

            try
            {
                _results.TrainingRestorationCreated = await restorer.CreateRestorationAsync(restorationRequest);
            }
            catch (Exception e)
            {
                _testContext.ThrownException = e;
            }
        }

        [When("I request the training restoration of environment '(.*)' (.*) anonymisation")]
        public async Task WhenIRequestTheTrainingRestorationOfEnvironmentWithOptions(string subdomain, bool anonymize)
        {
            var env = _instancesDbContext.Set<Environment>().Single(e => e.Subdomain == subdomain);
            _results.EnvironmentToRestore = env;

            var previousInstance = _instancesDbContext.Set<Instance>().SingleOrDefault(e => e.EnvironmentId == env.Id && e.IsActive);
            _results.PreviousTrainingInstanceId = previousInstance?.Id ?? null;

            var restorer = GetRestorer();
            var restorationRequest = new TrainingRestorationRequest
            {
                Anonymize = anonymize,
                Comment = "hello",
                CommentExpiryDate = null,
                EnvironmentId = env.Id,
                KeepExistingTrainingPasswords = true,
                RestoreFiles = true,
            };

            _results.TrainingRestorationRequest = restorationRequest;

            try
            {
                _results.TrainingRestorationCreated = await restorer.CreateRestorationAsync(restorationRequest);
            }
            catch (Exception e)
            {
                _testContext.ThrownException = e;
            }
        }


        [Then("the previous training instance should be marked as deleted")]
        public void ThenThePreviousTrainingInstanceShouldBeMarkedAsDeleted()
        {
            Assert.Contains(_results.PreviousTrainingInstanceId.Value, _results.DeletedInstanceIds);
        }

        [Then("we should not try to delete an instance")]
        public void ThenWeShouldNotTryToDeleteAnInstance()
        {
            Assert.Empty(_results.DeletedInstanceIds);
        }
        

        [Then("a backup of the previous training database should be made")]
        public void ThenABackupOfThePreviousTrainingDatabaseShouldBeMade()
        {
            Assert.Equal(_results.EnvironmentToRestore.Subdomain, _results.BackupRequestParameters.request.Tenant.Tenant);
            Assert.Equal(_results.EnvironmentToRestore.GetInstanceExecutingCluster(InstanceType.Training), _results.BackupRequestParameters.targetCluster);
        }

        [Then("the previous training should be marked as inactive")]
        public void ThenThePreviousTrainingShouldBeMarkedAsInactive()
        {
            Assert.False(_instancesDbContext.Set<Training>().SingleOrDefault(t => t.InstanceId == _results.PreviousTrainingInstanceId).IsActive);
        }

        [Then("a training restoration entry should be created")]
        public void ThenATrainingRestorationEntryShouldBeCreated()
        {
            Assert.NotNull(_instancesDbContext.Set<TrainingRestoration>().SingleOrDefault(tr => tr.Id == _results.TrainingRestorationCreated.Id));
        }

        [Then("a duplication of the production instance of the environment should be started")]
        public void ThenADuplicaationOfTheProductionInstanceOfTheEnvironmentShouldBeStarted()
        {
            // TODO : Regarder instructions plus bas
            Assert.Equal(_results.EnvironmentToRestore.Subdomain, _results.DuplicateRequestParameters.request.SourceTenant.Tenant);
            Assert.Equal(_results.EnvironmentToRestore.Subdomain, _results.DuplicateRequestParameters.request.TargetTenant);
            Assert.Equal(_results.EnvironmentToRestore.GetInstanceExecutingCluster(InstanceType.Training), _results.DuplicateRequestParameters.targetCluster);
        }

        [Then("the training restoration object stores the correct values")]
        public void ThenTheTrainingRestorationObjectStoresTheCorrectValues()
        {
            var trainingRestorationCreated = _instancesDbContext.Set<TrainingRestoration>().LastOrDefault();
            Assert.Equal(_results.TrainingRestorationRequest.Anonymize, trainingRestorationCreated.Anonymize);
            Assert.Equal(_results.TrainingRestorationRequest.KeepExistingTrainingPasswords, trainingRestorationCreated.KeepExistingTrainingPasswords);
            Assert.Equal(_results.TrainingRestorationRequest.RestoreFiles, trainingRestorationCreated.RestoreFiles);
            Assert.Equal(_results.TrainingRestorationRequest.CommentExpiryDate, trainingRestorationCreated.CommentExpiryDate);
            if (string.IsNullOrEmpty(_results.TrainingRestorationRequest.Comment))
            {
                Assert.True(string.IsNullOrEmpty(trainingRestorationCreated.Comment));
            }
            else
            {
                Assert.Equal(_results.TrainingRestorationRequest.Comment, trainingRestorationCreated.Comment);
            }
        }


        [Then("cleaning scripts should be applied")]
        public void ThenCleaningScriptsShouldBeApplied()
        {
            var allScripts = _results.DuplicateRequestParameters.request.PostBufferServerRestoreScripts.Select(prs => prs.Uri.ToString());
            Assert.All(_results.TrainingCleaningScriptsUri, (uri) => Assert.Contains(uri, allScripts));
        }

        [Then("anonymization scripts should be applied")]
        public void ThenAnonymizationScriptsShouldBeApplied()
        {
            var allScripts = _results.DuplicateRequestParameters.request.PostBufferServerRestoreScripts.Select(prs => prs.Uri.ToString());
            Assert.All(_results.AnonymizationScriptsUri, (uri) => Assert.Contains(uri, allScripts));
        }

        [Then("no anonymization scripts should be applied")]
        public void ThenNoAnonymizationScriptsShouldBeApplied()
        {
            var allScripts = _results.DuplicateRequestParameters.request.PostBufferServerRestoreScripts.Select(prs => prs.Uri.ToString());
            Assert.All(_results.AnonymizationScriptsUri, (uri) => Assert.DoesNotContain(uri, allScripts));
        }

        [Then("no other scripts should be applied on the buffer server")]
        public void ThenNoOtherScriptsShouldBeApplied()
        {
            var allScripts = _results.DuplicateRequestParameters.request.PostBufferServerRestoreScripts.Select(prs => prs.Uri.ToString());
            Assert.All(allScripts, (uri) => Assert.True(_results.TrainingCleaningScriptsUri.Contains(uri) || _results.AnonymizationScriptsUri.Contains(uri)));
        }

        private TrainingRestorer GetRestorer()
        {
            var codeSourcesStore = new CodeSourcesStore(_instancesDbContext, new DummyQueryPager());
            var githubBranchesStoreMock = new Mock<IGithubBranchesStore>();
            var codeSourceFetcherServiceMock = new Mock<ICodeSourceFetcherService>();
            var codeSourceUrlBuildServiceMock = new Mock<ICodeSourceBuildUrlService>();
            var artifactsServiceMock = new Mock<IArtifactsService>();
            var githubServiceMock = new Mock<IGithubService>();
            var previewConfigurationsRepositoryMock = new Mock<IPreviewConfigurationsRepository>();
            var githubRepoStoreMock = new Mock<IGithubReposStore>();


            var codeSourcesRepository = new CodeSourcesRepository(
                codeSourcesStore, githubBranchesStoreMock.Object,
                codeSourceFetcherServiceMock.Object, codeSourceUrlBuildServiceMock.Object,
                artifactsServiceMock.Object, githubServiceMock.Object,
                previewConfigurationsRepositoryMock.Object,
                githubRepoStoreMock.Object);

            var sqlScriptPicker = new SqlScriptPicker(codeSourcesRepository);

            var ccDataServiceMock = new Mock<ICcDataService>();
            ccDataServiceMock.Setup(ccds => ccds.CreateInstanceBackupAsync(It.IsAny<CreateInstanceBackupRequestDto>(), It.IsAny<string>()))
                .Callback((CreateInstanceBackupRequestDto backupRequest, string targetCluster) => _results.BackupRequestParameters = (backupRequest, targetCluster))
                .Returns(Task.CompletedTask);

            ccDataServiceMock.Setup(ccds => ccds.StartDuplicateInstanceAsync(It.IsAny<DuplicateInstanceRequestDto>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback((DuplicateInstanceRequestDto request, string targetCluster, string callbackUri) => _results.DuplicateRequestParameters = (request, targetCluster, callbackUri))
                .Returns(Task.CompletedTask);

            var instanceManipulator = new InstancesManipulator(sqlScriptPicker, ccDataServiceMock.Object);

            var trainingRestorationsStore = new TrainingRestorationsStore(_instancesDbContext);
            var trainingsStore = new TrainingsStore(_instancesDbContext, new DummyQueryPager());
            var environmentsStore = new EnvironmentsStore(_environmentsDbContext, new DummyQueryPager(), new Mock<IEnvironmentsRemoteStore>().Object);
            var rightsService = _testContext.GetRightsService();

            var remoteInstancesStoreMock = new Mock<IInstancesRemoteStore>();
            remoteInstancesStoreMock.Setup(ins => ins.DeleteByIdAsync(It.IsAny<int>())).Callback((int instanceId) =>
            {
                _results.DeletedInstanceIds.Add(instanceId);
            }).Returns(Task.CompletedTask);
            var instancesStore = new InstancesStore(_instancesDbContext, new DummyQueryPager(), remoteInstancesStoreMock.Object);

            return new TrainingRestorer(
                    _testContext.Principal,
                    instanceManipulator,
                    trainingRestorationsStore,
                    trainingsStore,
                    rightsService,
                    environmentsStore,
                    instancesStore);
        }

    }
}
