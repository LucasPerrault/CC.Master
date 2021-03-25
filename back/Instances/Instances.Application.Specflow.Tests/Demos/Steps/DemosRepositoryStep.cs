using Distributors.Domain;
using Distributors.Domain.Models;
using Environments.Domain.Storage;
using Instances.Application.Demos;
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
    public class DemosRepositoryStep
    {
        private readonly DemosContext _demosContext;

        public DemosRepositoryStep(DemosContext demosContext)
        {
            _demosContext = demosContext;

        }

        [When("I get the list of demos")]
        public async Task WhenIGetDemos()
        {
            var demoListQuery = new DemoListQuery()
            {
                IsActive = true,
                Page = null,
            };
            var demosStore = new DemosStore(_demosContext.DbContext, new DummyQueryPager());
            var rightsServiceMock = new Mock<IRightsService>();
            rightsServiceMock.Setup(rs => rs.GetUserOperationHighestScopeAsync(It.IsAny<Operation>())).ReturnsAsync((Operation op) => _demosContext.OperationsWithScope[op]);
            var demosRepository = new DemosRepository(_demosContext.Principal, demosStore, new DemoRightsFilter(rightsServiceMock.Object));
            _demosContext.DemosListResult = (await demosRepository.GetDemosAsync(demoListQuery)).Items.ToList();
        }

        [When("I request creation of demo '(.*)' by duplicating demo '(.*)' for distributor '(.*)'")]
        public async Task WhenICreateANewDemoByDuplicationForDistributor(string subdomain, string sourceSubdomain, string distributorId)
        {
            var demosStore = new DemosStore(_demosContext.DbContext, new DummyQueryPager());

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
            var dbDuplicatorMock = new Mock<ITenantDataDuplicator>();
            var passwordResetMock = new Mock<IDemoUsersPasswordResetService>();

            var duplicator = new DemoDuplicator
            (
                demosStore,
                instancesStoreMock.Object,
                rightsServiceMock.Object,
                distributorStoreMock.Object,
                new SubdomainValidator(demosStore, envStoreMock.Object),
                dbDuplicatorMock.Object,
                new UsersPasswordHelper(),
                new DemoRightsFilter(rightsServiceMock.Object),
                passwordResetMock.Object
            );

            var duplication = new DemoDuplication
            {
                Subdomain = subdomain,
                DistributorId = distributorId,
                Password = "test",
                SourceDemoSubdomain = sourceSubdomain,
                IsStrictSubdomainSelection = true
            };

            try
            {
                await duplicator.CreateDuplicationAsync(duplication, _demosContext.Principal);
            }
            catch (ForbiddenException e)
            {
                _demosContext.ExceptionResult = e;
            }
            catch (BadRequestException e)
            {
                _demosContext.ExceptionResult = e;
            }


            _demosContext.DemosListResult = demosStore.GetAllAsync().ToList();
        }

        [Then("it should contain (.*)")]
        public void ThenItShouldContainDemos(bool isTemplateDemo)
        {
            Assert.Contains(_demosContext.DemosListResult, d => d.IsTemplate == isTemplateDemo);
        }

        [Then("it should contain (.*) from '(.*)'")]
        public void ThenItShouldContainDemosFromDistributor(bool isTemplateDemo, string distributorCode)
        {
            Assert.Contains(_demosContext.DemosListResult, d => d.IsTemplate == isTemplateDemo && d.Distributor.Code == distributorCode);
        }

        [Then("it should not contain any (.*) from other distributors than '(.*)'")]
        public void ThenItShouldNotContainAnyDemosFromDistributorsOtherThan(bool isTemplateDemo, string distributorCode)
        {
            Assert.DoesNotContain(_demosContext.DemosListResult, d => d.Distributor.Code != distributorCode && d.IsTemplate == isTemplateDemo);
        }

        [Then(@"demo '(.*)' should exist for distributor '(.*)'")]
        public void ThenDemoShouldExistForDistributor(string subdomain, string distributorId)
        {
            Assert.Contains
            (
                _demosContext.DemosListResult,
                d => d.Subdomain == subdomain && d.DistributorID == distributorId
            );
        }

        [Then(@"demo '(.*)' should not exist")]
        public void ThenDemoShouldNotExistAsync(string subdomain)
        {
            Assert.DoesNotContain
            (
                _demosContext.DemosListResult,
                d => d.Subdomain == subdomain
            );
        }

        [StepArgumentTransformation(@"'(regular|template)' demos")]
        public bool IsTemplate(string value)
        {
            return value == "template";
        }
    }
}
