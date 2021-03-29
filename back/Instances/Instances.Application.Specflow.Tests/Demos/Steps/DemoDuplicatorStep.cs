using Distributors.Domain;
using Distributors.Domain.Models;
using Environments.Domain.Storage;
using Instances.Application.Demos;
using Instances.Application.Instances;
using Instances.Application.Specflow.Tests.Demos.Models;
using Instances.Application.Specflow.Tests.Shared.Tooling;
using Instances.Domain.Demos;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using Instances.Infra.DataDuplication;
using Instances.Infra.Demos;
using Instances.Infra.Instances.Services;
using Instances.Infra.Storage.Stores;
using Lucca.Core.Shared.Domain.Exceptions;
using Moq;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Xunit;

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


        [When("I request creation of demo '(.*)' by duplicating demo '(.*)' for distributor '(.*)'")]
        public async Task WhenICreateANewDemoByDuplicationForDistributor(string subdomain, string sourceSubdomain, string distributorId)
        {
            var demosStore = new DemosStore(_demosContext.DbContext, new DummyQueryPager());
            var demoDuplicationsStore = new DemoDuplicationsStore(_demosContext.DbContext, new DummyQueryPager());

            var distributorStoreMock = new Mock<IDistributorsStore>();
            distributorStoreMock.Setup(s => s.GetByCodeAsync(It.IsAny<string>()))
                .Returns<string>(distributor => Task.FromResult(new Distributor
                {
                    Id = distributor,
                    Code = distributor
                }));

            var instancesStoreMock = new Mock<IInstancesStore>();
            instancesStoreMock.Setup(s => s.CreateForDemoAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new Instance { Id = 1}));

            var rightsServiceMock = new Mock<IRightsService>();
            rightsServiceMock.Setup(rs => rs.GetUserOperationHighestScopeAsync(It.IsAny<Operation>()))
                .ReturnsAsync((Operation op) => _demosContext.OperationsWithScope[op]);

            var envStoreMock = new Mock<IEnvironmentsStore>();
            var passwordResetMock = new Mock<IDemoUsersPasswordResetService>();

            var instanceDuplicator = new InstancesDuplicator(new SqlScriptPicker());

            var duplicator = new DemoDuplicator
            (
                instanceDuplicator,
                demosStore,
                demoDuplicationsStore,
                instancesStoreMock.Object,
                rightsServiceMock.Object,
                distributorStoreMock.Object,
                new SubdomainValidator(demosStore, envStoreMock.Object),
                new UsersPasswordHelper(),
                new DemoRightsFilter(rightsServiceMock.Object),
                passwordResetMock.Object
            );

            var duplication = new DemoDuplicationRequest
            {
                Subdomain = subdomain,
                DistributorId = distributorId,
                Password = "test",
                SourceDemoSubdomain = sourceSubdomain
            };

            try
            {
                await duplicator.CreateDuplicationAsync
                    (duplication, DemoDuplicationRequestSource.Api, _demosContext.Principal);
            }
            catch (ForbiddenException e)
            {
                _demosContext.ExceptionResult = e;
            }
            catch (BadRequestException e)
            {
                _demosContext.ExceptionResult = e;
            }
        }

        [Then(@"demo duplication should exist for distributor '(.*)'")]
        public void ThenDemoDuplicationShouldExist(string distributorId)
        {
            Assert.Equal(distributorId, _demosContext.DbContext.Set<DemoDuplication>().Single().DistributorId);
        }
    }
}
