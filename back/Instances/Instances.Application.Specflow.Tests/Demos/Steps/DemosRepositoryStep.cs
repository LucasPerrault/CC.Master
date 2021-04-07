using Instances.Application.Demos;
using Instances.Application.Specflow.Tests.Demos.Models;
using Instances.Application.Specflow.Tests.Shared.Tooling;
using Instances.Domain.Demos;
using Instances.Domain.Instances;
using Instances.Infra.Storage.Stores;
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

        [Then(@"instance duplication should exist for subdomain '(.*)'")]
        public void ThenInstanceDuplicationShouldExistForSubdomainAsync(string subdomain)
        {
            Assert.Equal
                (
                    subdomain, _demosContext.DbContext.Set<InstanceDuplication>()
                        .Single()
                        .TargetSubdomain
                );
        }
    }
}
