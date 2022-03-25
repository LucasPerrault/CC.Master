using Distributors.Domain;
using Distributors.Domain.Models;
using Environments.Domain.Storage;
using FluentAssertions;
using Instances.Application.CodeSources;
using Instances.Application.Demos;
using Instances.Application.Demos.Duplication;
using Instances.Application.Instances;
using Instances.Application.Specflow.Tests.Demos.Models;
using Instances.Application.Specflow.Tests.Shared.Tooling;
using Instances.Domain.CodeSources;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Cleanup;
using Instances.Domain.Demos.Validation;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Domain.Shared;
using Instances.Infra.DataDuplication;
using Instances.Infra.Demos;
using Instances.Infra.Instances.Services;
using Instances.Infra.Storage;
using Instances.Infra.Storage.Stores;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Testing.Specflow;
using Tools;
using Xunit;
using Environment = Environments.Domain.Environment;

namespace Instances.Application.Specflow.Tests.Demos.Steps
{
    [Binding]
    public class DemoDuplicatorStep
    {
        private readonly SpecflowTestContext _testContext;
        private readonly InstancesDbContext _dbContext;
        private readonly DemoTestResults _results;

        public DemoDuplicatorStep(SpecflowTestContext testContext, InstancesDbContext dbContext, DemoTestResults results)
        {
            _testContext = testContext;
            _dbContext = dbContext;
            _results = results;
        }

        [Given(@"a (.*) duplication for demo '(.*)' of id '(.*)'")]
        public async Task GivenADuplication
        (
            InstanceDuplicationProgress progress,
            string subdomain,
            Guid duplicationId
        )
        {
            var distributor = _dbContext.Set<Distributor>().First();
            var demoDuplicationsStore = new DemoDuplicationsStore(_dbContext);
            var duplication = new DemoDuplication
            {
                Password = "test",
                InstanceDuplicationId = duplicationId,
                InstanceDuplication = new InstanceDuplication
                {
                    Id = duplicationId,
                    TargetSubdomain = subdomain,
                    Progress = progress,
                    DistributorId = distributor.Id,
                    SourceCluster = "mocked-source-cluster",
                    TargetCluster = "mocked-target-cluster",
                    SourceSubdomain = "mocked-source-subdomain"
                }
            };
            await demoDuplicationsStore.CreateAsync(duplication);
        }

        [Given(@"an existing demo '(.*)'")]
        public async Task GivenAnExistingDemo(string subdomain)
        {
            var demosStore = new DemosStore(_dbContext, new DummyQueryPager());

            var demo = new Demo
            {
                Subdomain = subdomain,
                IsActive = true,
                IsTemplate = false,
                InstanceID = 42,
                Instance = new Instance { Id = 42, IsActive = true },
                AuthorId = 42
            };

            await demosStore.CreateAsync(demo);
        }


        [When("I request creation of demo '(.*)' by duplicating demo '(.*)' (.*)")]
        public async Task WhenICreateANewDemoByDuplicationForDistributor(string subdomain, string sourceSubdomain, DistributorSelection selection)
        {
            var source = _dbContext.Set<Demo>().Single(d => d.Subdomain == sourceSubdomain);
            var duplicator = GetDuplicator();
            var duplication = new DemoDuplicationRequest
            {
                Subdomain = subdomain,
                DistributorCode = selection.Code,
                Password = "test",
                SourceId = source.Id
            };

            try
            {
                await duplicator.CreateDuplicationAsync(duplication);
            }
            catch (ForbiddenException e)
            {
                _testContext.ThrownException = e;
            }
            catch (BadRequestException e)
            {
                _testContext.ThrownException = e;
            }
        }

        [Then(@"demo duplication should exist (.*)")]
        public void ThenDemoDuplicationShouldExist(DistributorSelection selection)
        {
            var distributor = _dbContext.Set<Distributor>().Single(d => d.Code == selection.Code);
            Assert.Equal(distributor.Id, _dbContext.Set<DemoDuplication>().Single().DistributorId);
        }

        [When(@"I get notification that duplication '(.*)' has ended")]
        public async Task WhenIGetNotificationThatDuplicationHasEnded(Guid duplicationId)
        {
            var completer = GetCompleter(DemoCompleterSetup.HappyPath);
            await completer.MarkDuplicationAsCompletedAsync(duplicationId, true);
        }

        [When(@"duplication '(.*)' ends but password reset fails")]
        public async Task WhenDuplicationEndsButPasswordFails(Guid duplicationId)
        {
            var completer = GetCompleter(new DemoCompleterSetup { WillPasswordResetFail = true });
            await completer.MarkDuplicationAsCompletedAsync(duplicationId, true);
        }

        private DemoDuplicator GetDuplicator()
        {
            var demosStore = new DemosStore(_dbContext, new DummyQueryPager());
            var demoDuplicationsStore = new DemoDuplicationsStore(_dbContext);
            var instanceDuplicationsStore = new InstanceDuplicationsStore
            (
                _dbContext,
                new Mock<ITimeProvider>().Object
            );
            var envStoreMock = new Mock<IEnvironmentsStore>();
            envStoreMock
                .Setup(s => s.GetAsync(It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(new List<Environment>());

            var distributorsStoreMock = new Mock<IDistributorsStore>();

            distributorsStoreMock
                .Setup(s => s.GetActiveByCodeAsync(It.IsAny<string>()))
                .Returns<string>(distributor => Task.FromResult(_dbContext.Set<Distributor>().Single(d => d.Code == distributor)));

            var rightsService = _testContext.GetRightsService();

            var codeSourcesRepositoryMock = new Mock<ICodeSourcesRepository>();
            codeSourcesRepositoryMock.Setup(csr => csr.GetInstanceCleaningArtifactsAsync()).ReturnsAsync(new List<CodeSourceArtifacts>());
            codeSourcesRepositoryMock.Setup(csr => csr.GetMonolithArtifactsAsync()).ReturnsAsync(new List<CodeSourceArtifacts>());
            codeSourcesRepositoryMock.Setup(csr => csr.GetInstancePreRestoreArtifactsAsync()).ReturnsAsync(new List<CodeSourceArtifacts>());
            codeSourcesRepositoryMock.Setup(csr => csr.GetInstancePostRestoreArtifactsAsync()).ReturnsAsync(new List<CodeSourceArtifacts>());
            var ccDataServiceMock = new Mock<ICcDataService>();
            var clusterSelectorMock = new Mock<IClusterSelector>();
            clusterSelectorMock.Setup(s => s.GetFillingClusterAsync(It.IsAny<string>())).ReturnsAsync("mocked-cluster");
            var dnsMock = new Mock<IDnsService>();
            dnsMock.Setup(d => d.CreateAsync(It.IsAny<DnsEntry>()))
                .Returns<DnsEntry>(entry =>
                {
                    _results.SubdomainPropagations.Add(entry);
                    return Task.CompletedTask;
                });

            var subdomainValidationTranslator = new Mock<ISubdomainValidationTranslator>();
            var subdomainValidator = new SubdomainValidator(demosStore, envStoreMock.Object, instanceDuplicationsStore, subdomainValidationTranslator.Object);

            return new DemoDuplicator
                (
                    _testContext.Principal,
                    new InstancesManipulator(new SqlScriptPicker(codeSourcesRepositoryMock.Object),
                        ccDataServiceMock.Object
                    ),
                    demosStore,
                    demoDuplicationsStore,
                    rightsService,
                    distributorsStoreMock.Object,
                    new SubdomainGenerator(subdomainValidator),
                    clusterSelectorMock.Object,
                    new UsersPasswordHelper(),
                    dnsMock.Object
                );
        }

        private DemoDuplicationCompleter GetCompleter(DemoCompleterSetup setup)
        {
            var demosStore = new DemosStore(_dbContext, new DummyQueryPager());
            var demoDuplicationsStore = new DemoDuplicationsStore(_dbContext);
            var instanceDuplicationsStore = new InstanceDuplicationsStore
            (
                _dbContext,
                new Mock<ITimeProvider>().Object
            );

            var instancesStoreMock = new Mock<IInstancesStore>();
            var dnsMock = new Mock<IDnsService>();
            instancesStoreMock
                .Setup(s => s.CreateForDemoAsync(It.IsAny<string>()))
                .Callback<string>(
                    password =>
                    {
                        _results.CreatedInstances
                            .Add(new Instance { AllUsersImposedPassword = password });
                    })
                .Returns(Task.FromResult(new Instance { Id = 1 }));

            instancesStoreMock
                .Setup(s => s.DeleteByIdAsync(It.IsAny<int>()))
                .Callback<int>(
                    instanceId =>
                    {
                        _results.DeletedInstanceIds
                            .Add(instanceId);
                    })
                .Returns(Task.FromResult(new Instance { Id = 1}));

            var passwordResetServiceMock = new Mock<IDemoUsersPasswordResetService>();
            if (setup.WillPasswordResetFail)
            {
                passwordResetServiceMock
                    .Setup(s => s.ResetPasswordAsync(It.IsAny<Demo>(), It.IsAny<string>()))
                    .ThrowsAsync(new Exception("OUCH"));
            }


            return new DemoDuplicationCompleter
                (
                    demoDuplicationsStore,
                    instanceDuplicationsStore,
                    demosStore,
                    instancesStoreMock.Object,
                    new Mock<IWsAuthSynchronizer>().Object,
                    passwordResetServiceMock.Object,
                    new Mock<IDemoDeletionCalculator>().Object,
                    new Mock<ILogger<DemoDuplicationCompleter>>().Object,
                    dnsMock.Object
                );
        }

        [Then(@"duplication '(.*)' should result in instance creation")]
        public void ThenDuplicationShouldResultInInstanceCreation(Guid duplicationId)
        {
            Assert.Single(_results.CreatedInstances);
        }

        [Then(@"duplication '(.*)' should result in instance deletion")]
        public void ThenDuplicationShouldResultInInstanceDeletion(Guid duplicationId)
        {
            Assert.Single(_results.DeletedInstanceIds);
        }

        [Then(@"dns propagation should start (.*)")]
        public void ThenShouldPropagateOnDns(SubdomainSelection selection)
        {
            _results.SubdomainPropagations
                .Where(p => p.Subdomain == selection.Subdomain && p.Zone == DnsEntryZone.Demos)
                .Should().NotBeEmpty();
        }

        [Then(@"duplication '(.*)' should be marked as (.*)")]
        public void ThenDuplicationShouldResultInInstanceDeletion(Guid duplicationId, InstanceDuplicationProgress progress)
        {
            _dbContext.Set<InstanceDuplication>().Single(i => i.Id == duplicationId).Progress.Should().Be(progress);
        }

        [Then(@"(no|one) demo '(.*)' should be active")]
        public void ThenDuplicationShouldResultInInstanceDeletion(string demoExistenceKeyword, string subdomain)
        {
            var demos = _dbContext.Set<Demo>().Where(d => d.Subdomain == subdomain && d.IsActive);

            switch (demoExistenceKeyword)
            {
                case "no":
                    demos.Should().BeEmpty();
                    break;
                case "one":
                    demos.Should().HaveCount(1);
                    break;
            }
        }

        [Then(@"duplication '(.*)' should not result in instance deletion")]
        public void ThenDuplicationShouldNotResultInInstanceDeletion(Guid duplicationId)
        {
            Assert.Empty(_results.DeletedInstanceIds);
        }

        [Then(@"(no|one) duplication should be found as pending (.*)")]
        public async Task ThenThereShouldBeAPendingDuplicationForSubdomain(string duplicationCountKeyword, SubdomainSelection selection)
        {
            var instanceDuplicationsStore = new InstanceDuplicationsStore
            (
                _dbContext,
                new Mock<ITimeProvider>().Object
            );
            var pendingSubdomainDuplications = await instanceDuplicationsStore.GetPendingForSubdomainAsync(selection.Subdomain);

            switch (duplicationCountKeyword)
            {
                case "no":
                    pendingSubdomainDuplications.Should().BeEmpty();
                    break;
                case "one":
                    pendingSubdomainDuplications.Should().HaveCount(1);
                    break;
            }
        }

        private class DemoCompleterSetup
        {
            public bool WillPasswordResetFail { get; set; }
            public static DemoCompleterSetup HappyPath => new DemoCompleterSetup { WillPasswordResetFail = false };
        }
    }
}
