using Instances.Application.Demos;
using Instances.Application.Specflow.Tests.Demos.Models;
using Instances.Application.Specflow.Tests.Shared.Tooling;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Filtering;
using Instances.Domain.Instances;
using Instances.Domain.Shared;
using Instances.Infra.Storage.Stores;
using Moq;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System;
using System.Collections.Generic;
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

        [When(@"I delete (.*) demo (.*)")]
        public async Task WhenIDeleteDemoAsync(bool isTemplate, DistributorFilter filter)
        {
            var id = _demosContext.DbContext.Set<Demo>().Where(filter.AsFunc).First(d => d.IsTemplate == isTemplate).Id;
            await SafeDeleteAsync(id);
        }

        private async Task SafeDeleteAsync(int id)
        {
            var demosRepository = GetNewRepository();
            try
            {
                await demosRepository.DeleteAsync(id);
            }
            catch (Exception e)
            {
                _demosContext.ExceptionResult = e;
            }
        }

        [When("I get the list of demos")]
        public async Task WhenIGetDemos()
        {
            var demoListQuery = new DemoListQuery()
            {
                IsActive = new HashSet<bool>{ true },
                Page = null,
            };
            var demosRepository = GetNewRepository();
            _demosContext.DemosListResult = (await demosRepository.GetDemosAsync(demoListQuery)).Items.ToList();
        }

        [Then("it should contain (.*) demos")]
        public void ThenItShouldContainDemos(bool isTemplateDemo)
        {
            Assert.Contains(_demosContext.DemosListResult, d => d.IsTemplate == isTemplateDemo);
        }

        [Then("it should contain (.*) demos (.*)")]
        public void ThenItShouldContainDemosFromDistributor(bool isTemplateDemo, DistributorFilter filter)
        {
            Assert.Contains(_demosContext.DemosListResult, d => d.IsTemplate == isTemplateDemo && filter.AsFunc(d));
        }

        [Then("it should not contain any (.*) demos (.*)")]
        public void ThenItShouldNotContainAnyDemosFromDistributorsOtherThan(bool isTemplateDemo, DistributorFilter filter)
        {
            Assert.DoesNotContain(_demosContext.DemosListResult, d => d.IsTemplate == isTemplateDemo && filter.AsFunc(d));
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

        [Then(@" (.*) demo (.*) should not exist")]
        public void ThenDemoShouldNotExistFilteredAsync(bool isTemplateDemo, DistributorFilter filter)
        {
            Assert.DoesNotContain
                (
                    _demosContext.DemosListResult,
                    d => d.IsTemplate == isTemplateDemo && filter.AsFunc(d)
                );
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

        private DemosRepository GetNewRepository()
        {
            var demosStore = new DemosStore(_demosContext.DbContext, new DummyQueryPager());
            var rightsServiceMock = new Mock<IRightsService>();
            var instanceStoreMock = new Mock<IInstancesStore>();
            var ccDataServiceMock = new Mock<ICcDataService>();
            rightsServiceMock.Setup(rs => rs.GetUserOperationHighestScopeAsync(It.IsAny<Operation>())).ReturnsAsync((Operation op) => _demosContext.OperationsWithScope[op]);
            return new DemosRepository(_demosContext.Principal, demosStore, instanceStoreMock.Object, new DemoRightsFilter(rightsServiceMock.Object), ccDataServiceMock.Object);
        }
    }
}
