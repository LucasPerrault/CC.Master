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
using Rights.Domain.Filtering;
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
            var envStoreMock = new Mock<IEnvironmentsStore>();
            envStoreMock
                .Setup(s => s.GetAsync(It.IsAny<List<EnvironmentAccessRight>>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(new List<Environment>());

            var distributorsStoreMock = new Mock<IDistributorsStore>();

            distributorsStoreMock
                .Setup(s => s.GetByCodeAsync(It.IsAny<string>()))
                .Returns<string>(distributor => Task.FromResult(_demosContext.DbContext.Set<Distributor>().Single(d => d.Code == distributor)));

            var rightsServiceMock = new Mock<IRightsService>();
            rightsServiceMock.Setup(rs => rs.GetUserOperationHighestScopeAsync(It.IsAny<Operation>()))
                .ReturnsAsync((Operation op) => _demosContext.TestPrincipal.OperationsWithScope[op]);
            var ccDataServiceMock = new Mock<ICcDataService>();
            var clusterSelectorMock = new Mock<IClusterSelector>();

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
                    new SubdomainGenerator(new SubdomainValidator(demosStore, envStoreMock.Object)),
                    clusterSelectorMock.Object,
                    new UsersPasswordHelper()
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
            instancesStoreMock
                .Setup(s => s.CreateForDemoAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string,string>(
                    (password, cluster) =>
                    {
                        _demosContext.Results.CreatedInstances
                            .Add(new Instance { AllUsersImposedPassword = password, Cluster = cluster});
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
                    new Mock<ILogger<DemoDuplicationCompleter>>().Object
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

        private class DemoCompleterSetup
        {
            public bool WillPasswordResetFail { get; set; }
            public static DemoCompleterSetup HappyPath => new DemoCompleterSetup { WillPasswordResetFail = false };
        }
    }
}
