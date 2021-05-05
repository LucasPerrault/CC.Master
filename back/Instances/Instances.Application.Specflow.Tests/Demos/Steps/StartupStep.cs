using BoDi;
using Distributors.Domain.Models;
using Instances.Application.Specflow.Tests.Demos.Models;
using Instances.Domain.Demos;
using Instances.Domain.Instances.Models;
using Instances.Infra.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using TechTalk.SpecFlow;
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

        private static string GetNameUniqueForTestContext(string name)
        {
            return $"{name}-{Guid.NewGuid()}";
        }

        [BeforeScenario]
        public void InitializeScenario()
        {
            var demosContext = new DemosContext();
            _objectContainer.RegisterInstanceAs<DemosContext>(demosContext);

            var options = new DbContextOptionsBuilder<InstancesDbContext>()
                .UseInMemoryDatabase(GetNameUniqueForTestContext("Instances"))
                .Options;

            var context = new InstancesDbContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            demosContext.DbContext = context;
            var luccaDistributor = new Distributor()
            {
                Id = "1",
                Code = "LUCCA",
                Name = "Lucca",
            };
            context.Add(luccaDistributor);
            var otherDistributor = new Distributor()
            {
                Id = "2",
                Code = "DISTRIBUTOR",
                Name = "Other distributor",
            };

            var luccaUser = new SimpleUser
            {
                Id = 42,
                DepartmentId = 22,
                IsActive = true,
                FirstName = "Mia",
                LastName = "Houx"
            };

            context.Add(new Demo()
            {
                Id = 1,
                Subdomain = "virgin",
                IsActive = true,
                InstanceID = 1,
                DistributorID = luccaDistributor.Id,
                Distributor = luccaDistributor,
                IsTemplate = true,
                Instance = new Instance()
                {
                    Id = 1,
                    IsActive = true,
                    Cluster = "demo",
                    EnvironmentId = null,
                    Type = InstanceType.Demo
                },
                AuthorId = luccaUser.Id,
                Author = luccaUser
            });
            context.Add(new Demo()
            {
                Id = 2,
                Subdomain = "demo-lucca",
                InstanceID = 2,
                IsActive = true,
                DistributorID = luccaDistributor.Id,
                Distributor = luccaDistributor,
                IsTemplate = false,
                Instance = new Instance()
                {
                    Id = 2,
                    IsActive = true,
                    Cluster = "demo",
                    EnvironmentId = null,
                    Type = InstanceType.Demo
                },
                AuthorId = luccaUser.Id,
                Author = luccaUser
            });
            context.Add(new Demo()
            {
                Id = 3,
                Subdomain = "demo-distributor",
                InstanceID = 3,
                IsActive = true,
                DistributorID = otherDistributor.Id,
                Distributor = otherDistributor,
                IsTemplate = false,
                Instance = new Instance()
                {
                    Id = 3,
                    IsActive = true,
                    Cluster = "demo",
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
