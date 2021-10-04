using BoDi;
using Distributors.Domain.Models;
using Instances.Application.Specflow.Tests.Demos.Models;
using Instances.Domain.Demos;
using Instances.Domain.Instances.Models;
using Instances.Infra.Storage;
using TechTalk.SpecFlow;
using Testing.Infra;
using Users.Domain;

namespace Instances.Application.Specflow.Tests.Demos.Steps
{
    [Binding]
    public class StartupStep
    {
        private readonly IObjectContainer _objectContainer;

        public StartupStep(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeScenario]
        public void InitializeScenario()
        {
            var demosContext = new DemosContext();
            _objectContainer.RegisterInstanceAs(demosContext);
            _objectContainer.RegisterInstanceAs(demosContext.Results.ExceptionResult);

            demosContext.DbContext = InMemoryDbHelper.InitialiseDb<InstancesDbContext>("Instances", o => new InstancesDbContext(o));
            var luccaDistributor = new Distributor
            {
                Id = 1,
                Code = "LUCCA",
                Name = "Lucca",
            };
            demosContext.DbContext.Add(luccaDistributor);
            var otherDistributor = new Distributor()
            {
                Id = 2,
                Code = "DISTRIBUTOR",
                Name = "Other distributor",
            };

            var luccaUser = new SimpleUser
            {
                Id = 42,
                IsActive = true,
                FirstName = "Mia",
                LastName = "Houx"
            };

            demosContext.DbContext.Add(new Demo()
            {
                Id = 1,
                Subdomain = "virgin",
                IsActive = true,
                InstanceID = 1,
                DistributorId = luccaDistributor.Id,
                Distributor = luccaDistributor,
                IsTemplate = true,
                Cluster = "demo",
                Instance = new Instance()
                {
                    Id = 1,
                    IsActive = true,
                    EnvironmentId = null,
                    Type = InstanceType.Demo
                },
                AuthorId = luccaUser.Id,
                Author = luccaUser
            });
            demosContext.DbContext.Add(new Demo()
            {
                Id = 2,
                Subdomain = "demo-lucca",
                InstanceID = 2,
                IsActive = true,
                DistributorId = luccaDistributor.Id,
                Distributor = luccaDistributor,
                IsTemplate = false,
                Cluster = "demo",
                Instance = new Instance()
                {
                    Id = 2,
                    IsActive = true,
                    EnvironmentId = null,
                    Type = InstanceType.Demo
                },
                AuthorId = luccaUser.Id,
                Author = luccaUser
            });
            demosContext.DbContext.Add(new Demo()
            {
                Id = 3,
                Subdomain = "demo-distributor",
                InstanceID = 3,
                IsActive = true,
                DistributorId = otherDistributor.Id,
                Distributor = otherDistributor,
                IsTemplate = false,
                Cluster = "demo",
                Instance = new Instance()
                {
                    Id = 3,
                    IsActive = true,
                    EnvironmentId = null,
                    Type = InstanceType.Demo
                },
                AuthorId = luccaUser.Id,
                Author = luccaUser
            });
            demosContext.DbContext.SaveChanges();
        }

    }
}
