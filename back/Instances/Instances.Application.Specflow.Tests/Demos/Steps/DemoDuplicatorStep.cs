using Distributors.Domain;
using Distributors.Domain.Models;
using Environments.Domain.Storage;
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
            Assert.Equal(selection.Code, _demosContext.DbContext.Set<DemoDuplication>().Single().DistributorId);
        }

        [When(@"I get notification that duplication '(.*)' has ended")]
        public async Task WhenIGetNotificationThatDuplicationHasEnded(Guid duplicationId)
        {
            var completer = GetCompleter();
            await completer.MarkDuplicationAsCompletedAsync(duplicationId, true);
        }

        private DemoDuplicator GetDuplicator()
        {
            var demosStore = new DemosStore(_demosContext.DbContext, new DummyQueryPager());
            var demoDuplicationsStore = new DemoDuplicationsStore(_demosContext.DbContext);
            var envStoreMock = new Mock<IEnvironmentsStore>();
            envStoreMock
                .Setup(s => s.GetFilteredAsync(It.IsAny<AccessRight>(), It.IsAny<PurposeAccessRight>(), It.IsAny<EnvironmentFilter>()))
                .ReturnsAsync(new List<Environment>());

            var distributorsStoreMock = new Mock<IDistributorsStore>();

            distributorsStoreMock
                .Setup(s => s.GetByCodeAsync(It.IsAny<string>()))
                .Returns<string>(distributor => Task.FromResult(new Distributor
                {
                    Id = distributor,
                    Code = distributor
                }));

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

        private DemoDuplicationCompleter GetCompleter()
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
                .Returns(Task.FromResult(new Instance { Id = 1}));

            var rightsServiceMock = new Mock<IRightsService>();
            rightsServiceMock.Setup(rs => rs.GetUserOperationHighestScopeAsync(It.IsAny<Operation>()))
                .ReturnsAsync((Operation op) => _demosContext.TestPrincipal.OperationsWithScope[op]);

            return new DemoDuplicationCompleter
                (
                    demoDuplicationsStore,
                    instanceDuplicationsStore,
                    demosStore,
                    instancesStoreMock.Object,
                    new Mock<IWsAuthSynchronizer>().Object,
                    new Mock<IDemoUsersPasswordResetService>().Object,
                    new Mock<IDemoDeletionCalculator>().Object,
                    new Mock<ILogger<DemoDuplicationCompleter>>().Object
                );
        }

        [Then(@"duplication '(.*)' should result in instance creation")]
        public void ThenDuplicationShouldResultInInstanceCreation(Guid duplicationId)
        {
            Assert.Single(_demosContext.Results.CreatedInstances);
        }
    }
}
