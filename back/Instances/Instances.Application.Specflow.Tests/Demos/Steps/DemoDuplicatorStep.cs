using Distributors.Domain;
using Distributors.Domain.Models;
using Environments.Domain.Storage;
using FluentAssertions;
using Instances.Application.Demos;
using Instances.Application.Demos.Duplication;
using Instances.Application.Instances;
using Instances.Application.Specflow.Tests.Demos.Models;
using Instances.Application.Specflow.Tests.Shared.Tooling;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Cleanup;
using Instances.Domain.Demos.Validation;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Domain.Shared;
using Instances.Infra.DataDuplication;
using Instances.Infra.Demos;
using Instances.Infra.Instances.Services;
using Instances.Infra.Storage.Stores;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Tools;
using Xunit;
using Environment = Environments.Domain.Environment;

namespace Instances.Application.Specflow.Tests.Demos.Steps
{
    [Binding]
    public class DemoDuplicatorStep
    {
        private readonly DemosContext _demosContext;

        public DemoDuplicatorStep(DemosContext demosContext)
        {
            _demosContext = demosContext;
        }

        [Given(@"a (.*) duplication for demo '(.*)' of id '(.*)'")]
        public async Task GivenADuplication
        (
            InstanceDuplicationProgress progress,
            string subdomain,
            Guid duplicationId
        )
        {
            var distributor = _demosContext.DbContext.Set<Distributor>().First();
            var demoDuplicationsStore = new DemoDuplicationsStore(_demosContext.DbContext);
            var duplication = new DemoDuplication
            {
                Password = "test",
                InstanceDuplicationId = duplicationId,
                InstanceDuplication = new InstanceDuplication
                {
                    Id = duplicationId,
                    TargetSubdomain = subdomain,
                    Progress = progress,
                    DistributorId = distributor.Id
                }
            };
            await demoDuplicationsStore.CreateAsync(duplication);
        }

        [Given(@"an existing demo '(.*)'")]
        public async Task GivenAnExistingDemo(string subdomain)
        {
            var demosStore = new DemosStore(_demosContext.DbContext, new DummyQueryPager());

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
            var source = _demosContext.DbContext.Set<Demo>().Single(d => d.Subdomain == sourceSubdomain);
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
                _demosContext.Results.ExceptionResult.Exception = e;
            }
            catch (BadRequestException e)
            {
                _demosContext.Results.ExceptionResult.Exception = e;
            }
        }

        [Then(@"demo duplication should exist (.*)")]
        public void ThenDemoDuplicationShouldExist(DistributorSelection selection)
        {
            var distributor = _demosContext.DbContext.Set<Distributor>().Single(d => d.Code == selection.Code);
            Assert.Equal(distributor.Id, _demosContext.DbContext.Set<DemoDuplication>().Single().DistributorId);
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
            var demosStore = new DemosStore(_demosContext.DbContext, new DummyQueryPager());
            var demoDuplicationsStore = new DemoDuplicationsStore(_demosContext.DbContext);
            var instanceDuplicationsStore = new InstanceDuplicationsStore
            (
                _demosContext.DbContext,
                new Mock<ITimeProvider>().Object
            );
            var envStoreMock = new Mock<IEnvironmentsStore>();
            envStoreMock
                .Setup(s => s.GetAsync(It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(new List<Environment>());

            var distributorsStoreMock = new Mock<IDistributorsStore>();

            distributorsStoreMock
                .Setup(s => s.GetActiveByCodeAsync(It.IsAny<string>()))
                .Returns<string>(distributor => Task.FromResult(_demosContext.DbContext.Set<Distributor>().Single(d => d.Code == distributor)));

            var rightsServiceMock = new Mock<IRightsService>();
            rightsServiceMock.Setup(rs => rs.GetUserOperationHighestScopeAsync(It.IsAny<Operation>()))
                .ReturnsAsync((Operation op) => _demosContext.TestPrincipal.OperationsWithScope[op]);
            var ccDataServiceMock = new Mock<ICcDataService>();
            var clusterSelectorMock = new Mock<IClusterSelector>();
            var dnsMock = new Mock<IDnsService>();
            dnsMock.Setup(d => d.CreateAsync(It.IsAny<DnsEntry>()))
                .Returns<DnsEntry>(entry =>
                {
                    _demosContext.Results.SubdomainPropagations.Add(entry);
                    return Task.CompletedTask;
                });

            var subdomainValidationTranslator = new Mock<ISubdomainValidationTranslator>();
            var subdomainValidator = new SubdomainValidator(demosStore, envStoreMock.Object, instanceDuplicationsStore, subdomainValidationTranslator.Object);

            return new DemoDuplicator
                (
                    _demosContext.TestPrincipal.Principal,
                    new InstancesDuplicator(new SqlScriptPicker(
                        new SqlScriptPickerConfiguration
                        {
                            JenkinsBaseUri = new Uri("http://localhost"),
                            MonolithJobPath = "ilucca",
                        }),
                        ccDataServiceMock.Object
                    ),
                    demosStore,
                    demoDuplicationsStore,
                    rightsServiceMock.Object,
                    distributorsStoreMock.Object,
                    new SubdomainGenerator(subdomainValidator),
                    clusterSelectorMock.Object,
                    new UsersPasswordHelper(),
                    dnsMock.Object
                );
        }

        private DemoDuplicationCompleter GetCompleter(DemoCompleterSetup setup)
        {
            var demosStore = new DemosStore(_demosContext.DbContext, new DummyQueryPager());
            var demoDuplicationsStore = new DemoDuplicationsStore(_demosContext.DbContext);
            var instanceDuplicationsStore = new InstanceDuplicationsStore
            (
                _demosContext.DbContext,
                new Mock<ITimeProvider>().Object
            );

            var instancesStoreMock = new Mock<IInstancesStore>();
            var dnsMock = new Mock<IDnsService>();
            instancesStoreMock
                .Setup(s => s.CreateForDemoAsync(It.IsAny<string>()))
                .Callback<string>(
                    password =>
                    {
                        _demosContext.Results.CreatedInstances
                            .Add(new Instance { AllUsersImposedPassword = password });
                    })
                .Returns(Task.FromResult(new Instance { Id = 1 }));

            instancesStoreMock
                .Setup(s => s.DeleteForDemoAsync(It.IsAny<Instance>()))
                .Callback<Instance>(
                    instance =>
                    {
                        _demosContext.Results.DeleteInstances
                            .Add(instance);
                    })
                .Returns(Task.FromResult(new Instance { Id = 1}));

            var rightsServiceMock = new Mock<IRightsService>();
            rightsServiceMock.Setup(rs => rs.GetUserOperationHighestScopeAsync(It.IsAny<Operation>()))
                .ReturnsAsync((Operation op) => _demosContext.TestPrincipal.OperationsWithScope[op]);

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
            Assert.Single(_demosContext.Results.CreatedInstances);
        }

        [Then(@"duplication '(.*)' should result in instance deletion")]
        public void ThenDuplicationShouldResultInInstanceDeletion(Guid duplicationId)
        {
            Assert.Single(_demosContext.Results.DeleteInstances);
        }

        [Then(@"dns propagation should start (.*)")]
        public void ThenShouldPropagateOnDns(SubdomainSelection selection)
        {
            _demosContext.Results.SubdomainPropagations
                .Where(p => p.Subdomain == selection.Subdomain && p.Zone == DnsEntryZone.Demos)
                .Should().NotBeEmpty();
        }

        [Then(@"duplication '(.*)' should be marked as (.*)")]
        public void ThenDuplicationShouldResultInInstanceDeletion(Guid duplicationId, InstanceDuplicationProgress progress)
        {
            _demosContext.DbContext.Set<InstanceDuplication>().Single(i => i.Id == duplicationId).Progress.Should().Be(progress);
        }

        [Then(@"(no|one) demo '(.*)' should be active")]
        public void ThenDuplicationShouldResultInInstanceDeletion(string demoExistenceKeyword, string subdomain)
        {
            var demos = _demosContext.DbContext.Set<Demo>().Where(d => d.Subdomain == subdomain && d.IsActive);

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
            Assert.Empty(_demosContext.Results.DeleteInstances);
        }

        [Then(@"(no|one) duplication should be found as pending (.*)")]
        public async Task ThenThereShouldBeAPendingDuplicationForSubdomain(string duplicationCountKeyword, SubdomainSelection selection)
        {
            var instanceDuplicationsStore = new InstanceDuplicationsStore
            (
                _demosContext.DbContext,
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
