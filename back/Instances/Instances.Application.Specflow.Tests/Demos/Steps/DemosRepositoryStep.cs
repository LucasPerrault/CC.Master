using Instances.Application.Demos;
using Instances.Application.Specflow.Tests.Demos.Models;
using Instances.Application.Specflow.Tests.Shared.Tooling;
using Instances.Domain.Demos;
using Instances.Infra.Storage.Stores;
using Moq;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        [Then("it should contain a '(.*)' demo from '(.*)'")]
        public void ThenItShouldContainADemo(string demoName, string distributorCode)
        {
            Assert.Contains(_demosContext.DemosListResult, d => d.Subdomain == demoName && d.Distributor.Code == distributorCode);
        }

        [Then("it should not contain any demo from other distributors than '(.*)'")]
        public void ThenItShouldNotContainAnyOtherDemo(string distributorCode)
        {
            Assert.DoesNotContain(_demosContext.DemosListResult, d => d.Distributor.Code != distributorCode);
        }
    }
}
