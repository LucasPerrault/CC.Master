using Environments.Domain;
using Environments.Infra.Storage;
using Environments.Infra.Storage.Stores;
using Instances.Application.CodeSources;
using Instances.Application.Instances;
using Instances.Application.Specflow.Tests.Shared.Tooling;
using Instances.Application.Specflow.Tests.Trainings.Models;
using Instances.Application.Trainings;
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
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Testing.Specflow;
using Tools;
using Xunit;

namespace Instances.Application.Specflow.Tests.Trainings.Steps
{
    [Binding]
    public class TrainingRestorationEndStep
    {
        private readonly SpecflowTestContext _testContext;
        private readonly InstancesDbContext _instancesDbContext;
        private readonly EnvironmentsDbContext _environmentsDbContext;
        private readonly TrainingRestorationEndTestResults _results;
        private Mock<IInstancesRemoteStore> _remoteInstancesStoreMock;
        private Mock<IWsAuthSynchronizer> _wsAuthSynchronizerMock;
        private Mock<ICcDataService> _ccDataServiceMock;

        public TrainingRestorationEndStep(SpecflowTestContext testContext, InstancesDbContext instancesDbContext, EnvironmentsDbContext environmentsDbContext, TrainingRestorationEndTestResults results)
        {
            _testContext = testContext;
            _instancesDbContext = instancesDbContext;
            _environmentsDbContext = environmentsDbContext;
            _results = results;
        }

        [Given("a (.*) training restoration")]
        public void GivenATrainingRestorationWithId(InstanceDuplicationProgress progress)
        {
            _results.GivenRestoration = _instancesDbContext.Set<TrainingRestoration>().FirstOrDefault(tr => tr.InstanceDuplication.Progress == progress);
        }

        [When("I mark the restoration as completed (.*) success")]
        public async Task WhenIMarkTheRestorationAsCompleted(bool successful)
        {
            var completer = GetCompleter();
            try
            {
                await completer.MarkRestorationAsCompletedAsync(_results.GivenRestoration.InstanceDuplicationId, successful);
            }
            catch (Exception e)
            {
                _testContext.ThrownException = e;
            }
        }

        [When("I mark the restoration as completed but there is an error")]
        public async Task WhenIMarkTheRestorationAsCompletedButThereIsAnError()
        {
            var completer = GetCompleter();
            _remoteInstancesStoreMock.Setup(ris => ris.CreateForTrainingAsync(It.IsAny<int>(), It.IsAny<bool>())).ThrowsAsync(new Exception("Dummy Exception"));
            await completer.MarkRestorationAsCompletedAsync(_results.GivenRestoration.InstanceDuplicationId, true);
        }

        [Then("the restoration should be marked as (.*)")]
        public void ThenTheRestorationShouldBeMarkedAs(InstanceDuplicationProgress progress)
        {
            Assert.Equal(progress, _results.GivenRestoration.InstanceDuplication.Progress);
        }

        [Then("a log should be created with (.*)")]
        public void ThenALogShouldBeCreatedWith(EnvironmentLogActivity logActivity)
        {
            Assert.NotNull(_environmentsDbContext.Set<EnvironmentLog>().FirstOrDefault(el => el.EnvironmentId == _results.GivenRestoration.EnvironmentId && el.ActivityId == logActivity));
        }

        [Then("an instance of type training is created")]
        public void ThenAnInstanceOfTypeTrainingIsCreated()
        {
            _remoteInstancesStoreMock.Verify(ris => ris.CreateForTrainingAsync(It.Is<int>(i => i == _results.GivenRestoration.EnvironmentId), It.Is<bool>(b => b == _results.GivenRestoration.Anonymize)), Times.Once);
        }

        [Then("a training object is created")]
        public void ThenATrainingObjectIsCreated()
        {
            Assert.NotNull(_instancesDbContext.Set<Training>().FirstOrDefault(
                t =>
                t.EnvironmentId == _results.GivenRestoration.EnvironmentId
                && t.InstanceId == _results.CreatedInstance.Id
                && t.TrainingRestorationId == _results.GivenRestoration.Id));
        }

        [Then("a synchronization with WsAuth is launched")]
        public void ThenASynchronizationWithWsAuthIsLaunched()
        {
            _wsAuthSynchronizerMock.Verify(wsas => wsas.SafeSynchronizeAsync(It.Is<int>(i => i == _results.CreatedInstance.Id)), Times.Once);

        }

        [Then("the cache of the training instance is reset")]
        public void ThenTheCacheOfTheTrainingInstanceIsReset()
        {
            var environment = _results.GivenRestoration.Environment;
            _ccDataServiceMock.Verify(ccds => ccds.ResetInstanceCacheAsync(It.Is<ResetInstanceCacheRequestDto>(ricr => ricr.TenantHost == environment.GetInstanceHost(InstanceType.Training)), It.Is<string>(s => s == environment.GetInstanceExecutingCluster(InstanceType.Training))), Times.Once);

        }

        private TrainingRestorationCompleter GetCompleter()
        {
            var trainingRestorationsStore = new TrainingRestorationsStore(_instancesDbContext);
            var instanceDuplicationsStore = new InstanceDuplicationsStore(_instancesDbContext, new Mock<ITimeProvider>().Object);
            var trainingsStore = new TrainingsStore(_instancesDbContext, new DummyQueryPager());
            _remoteInstancesStoreMock = new Mock<IInstancesRemoteStore>();

            _results.CreatedInstance = new Instance
            {
                Id = 4242,
                IsActive = true,
                EnvironmentId = _results.GivenRestoration.EnvironmentId,
                Type = InstanceType.Training,
                AllUsersImposedPassword = null,
                IsAnonymized = _results.GivenRestoration.Anonymize,
                IsProtected = false,
            };
            _remoteInstancesStoreMock.Setup(ris => ris.CreateForTrainingAsync(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(_results.CreatedInstance);

            var instancesStore = new InstancesStore(_instancesDbContext, new DummyQueryPager(), _remoteInstancesStoreMock.Object);
            var environmentLogsStore = new EnvironmentLogsStore(_environmentsDbContext);
            var trainingRestorationLogsRepository = new TrainingRestorationLogsRepository(environmentLogsStore);

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

            _ccDataServiceMock = new Mock<ICcDataService>();
            _ccDataServiceMock.Setup(ccds => ccds.ResetInstanceCacheAsync(It.IsAny<ResetInstanceCacheRequestDto>(), It.IsAny<string>())).ReturnsAsync(true);

            var instanceManipulator = new InstancesManipulator(sqlScriptPicker, _ccDataServiceMock.Object);
            _wsAuthSynchronizerMock = new Mock<IWsAuthSynchronizer>();
            _wsAuthSynchronizerMock.Setup(wsas => wsas.SafeSynchronizeAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            return new TrainingRestorationCompleter(
                trainingRestorationsStore,
                instanceDuplicationsStore,
                trainingsStore,
                instancesStore,
                trainingRestorationLogsRepository,
                _wsAuthSynchronizerMock.Object,
                instanceManipulator,
                new Mock<ILogger<TrainingRestorationCompleter>>().Object
                );

        }
    }
}
