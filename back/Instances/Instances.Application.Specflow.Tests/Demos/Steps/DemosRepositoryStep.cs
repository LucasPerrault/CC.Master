using Distributors.Domain.Models;
using Instances.Application.Demos;
using Instances.Application.Demos.Dtos;
using Instances.Application.Specflow.Tests.Demos.Models;
using Instances.Application.Specflow.Tests.Shared.Tooling;
using Instances.Domain.Demos;
using Instances.Domain.Demos.Filtering;
using Instances.Domain.Instances;
using Instances.Domain.Shared;
using Instances.Infra.Storage;
using Instances.Infra.Storage.Stores;
using Moq;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Testing.Specflow;
using Tools;
using Tools.Web;
using Xunit;

namespace Instances.Application.Specflow.Tests.Demos.Steps
{
    [Binding]
    public class DemosRepositoryStep
    {
        private readonly SpecflowTestContext _testContext;
        private readonly DemoTestResults _results;
        private readonly InstancesDbContext _dbContext;

        public DemosRepositoryStep(SpecflowTestContext testContext, DemoTestResults results, InstancesDbContext dbContext)
        {
            _testContext = testContext;
            _results = results;
            _dbContext = dbContext;
        }

        [When("I get the list of (.*) demos")]
        public async Task WhenIGetDemos(bool isTemplate)
        {
            var demoFilter = new DemoFilter()
            {
                IsTemplate = new HashSet<bool> { isTemplate }.ToCompareBoolean(),
            };
            var demosRepository = GetNewRepository();
            var demoPage = await demosRepository.GetDemosAsync(null, demoFilter);
            _results.Demos.AddRange(demoPage.Items);
        }

        [When("I get the list of demos (.*)")]
        public async Task WhenIGetDemosForSubdomain(SubdomainSelection selection)
        {
            var demoFilter = new DemoFilter()
            {
                Subdomain = CompareString.Equals(selection.Subdomain),
            };
            var demosRepository = GetNewRepository();
            var demoPage = await demosRepository.GetDemosAsync(null, demoFilter);
            _results.Demos.AddRange(demoPage.Items);
        }

        [When("I update the comment to '(.*)' on (.*) demo (.*)")]
        public async Task WhenIUpdateDemoComment(string comment, bool isTemplate, DistributorFilter filter)
        {
            var demoPutPayload = new DemoPutPayload()
            {
                Comment = comment,
            };
            var id = _dbContext.Set<Demo>().Where(filter.AsFunc).First(d => d.IsTemplate == isTemplate).Id;
            await SafePutAsync(id, demoPutPayload);
        }

        private async Task SafePutAsync(int id, DemoPutPayload payload)
        {
            var demosRepository = GetNewRepository();
            try
            {
                _results.SingleDemo = await demosRepository.UpdateDemoAsync(id, payload);
            }
            catch (Exception e)
            {
                _testContext.ThrownException = e;
            }
        }

        [When(@"I delete (.*) demo (.*)")]
        public async Task WhenIDeleteDemoAsync(bool isTemplate, DistributorFilter filter)
        {
            var id = _dbContext.Set<Demo>().Where(filter.AsFunc).First(d => d.IsTemplate == isTemplate).Id;
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
                _testContext.ThrownException = e;
            }
        }

        [When("I get the list of demos")]
        public async Task WhenIGetDemos()
        {
            var demoFilter = new DemoFilter()
            {
                IsActive = CompareBoolean.TrueOnly,
            };
            var demosRepository = GetNewRepository();
            var demoPage = await demosRepository.GetDemosAsync(null, demoFilter);
            _results.Demos.AddRange(demoPage.Items);
        }

        [Then("it should contain (.*) demos")]
        public void ThenItShouldContainDemos(bool isTemplateDemo)
        {
            Assert.Contains(_results.Demos, d => d.IsTemplate == isTemplateDemo);
        }

        [Then("it should contain (.*) demos (.*)")]
        public void ThenItShouldContainDemosFromDistributor(bool isTemplateDemo, DistributorFilter filter)
        {
            Assert.Contains(_results.Demos, d => d.IsTemplate == isTemplateDemo && filter.AsFunc(d));
        }

        [Then("it should not contain any (.*) demos (.*)")]
        public void ThenItShouldNotContainAnyDemosFromDistributorsOtherThan(bool isTemplateDemo, DistributorFilter filter)
        {
            Assert.DoesNotContain(_results.Demos, d => d.IsTemplate == isTemplateDemo && filter.AsFunc(d));
        }

        [Then(@"demo '(.*)' should exist for distributor '(.*)'")]
        public void ThenDemoShouldExistForDistributor(string subdomain, string distributorCode)
        {
            var distributor = _dbContext.Set<Distributor>().Single(d => d.Code == distributorCode);
            Assert.Contains
            (
                _results.Demos,
                d => d.Subdomain == subdomain && d.DistributorId == distributor.Id
            );
        }

        [Then("demo comment should be '(.*)'")]
        public void ThenDemoCommentShouldBe(string comment)
        {
            Assert.Equal(comment, _results.SingleDemo.Comment);
        }

        [Then("it should contain demos (.*)")]
        public void ThenItShouldContainDemosForSubdomain(SubdomainSelection selection)
        {
            Assert.Contains(_results.Demos, d => d.Subdomain == selection.Subdomain);
        }

        [Then("it should not contain any demo other than '(.*)'")]
        public void ThenItShouldNotContainDemosWithSubdomainOtherThan(string subdomain)
        {
            Assert.DoesNotContain(_results.Demos, d => d.Subdomain != subdomain);
        }

        [Then(@"demo '(.*)' should not exist")]
        public void ThenDemoShouldNotExistAsync(string subdomain)
        {
            Assert.DoesNotContain
                (
                    _results.Demos,
                    d => d.Subdomain == subdomain
                );
        }

        [Then(@" (.*) demo (.*) should not exist")]
        public void ThenDemoShouldNotExistFilteredAsync(bool isTemplateDemo, DistributorFilter filter)
        {
            Assert.DoesNotContain
                (
                    _results.Demos,
                    d => d.IsTemplate == isTemplateDemo && filter.AsFunc(d)
                );
        }

        [Then(@"instance duplication should exist (.*)")]
        public void ThenInstanceDuplicationShouldExistForSubdomainAsync(SubdomainSelection selection)
        {
            Assert.Equal
                (
                    selection.Subdomain, _dbContext.Set<InstanceDuplication>()
                        .Single()
                        .TargetSubdomain
                );
        }

        private DemosRepository GetNewRepository()
        {
            var demosStore = new DemosStore(_dbContext, new DummyQueryPager());
            var instanceStoreMock = new Mock<IInstancesStore>();
            var ccDataServiceMock = new Mock<ICcDataService>();
            var dnsServiceMock = new Mock<IDnsService>();
            var rightsService = _testContext.GetRightsService();
            return new DemosRepository(_testContext.Principal, demosStore, instanceStoreMock.Object, new DemoRightsFilter(new RightsFilter(rightsService)), ccDataServiceMock.Object, dnsServiceMock.Object);
        }
    }
}
