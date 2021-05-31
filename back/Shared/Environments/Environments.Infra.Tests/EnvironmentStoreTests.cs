using Environments.Domain;
using Environments.Domain.Storage;
using Environments.Infra.Storage;
using Environments.Infra.Storage.Stores;
using Rights.Domain.Filtering;
using System.Collections.Generic;
using System.Threading.Tasks;
using Testing.Infra;
using Xunit;

namespace Environments.Infra.Tests
{
    public class EnvironmentStoreTests
    {
        [Fact]
        public async Task ShouldReturnEnvironmentWithoutAccessRights()
        {
            var dbContext = InMemoryDbHelper.InitialiseDb<EnvironmentsDbContext>("Mocked", o => new EnvironmentsDbContext(o));
            dbContext.Add
            (
                new Environment
                {
                    Purpose = EnvironmentPurpose.Lucca,
                    ActiveAccesses = new List<EnvironmentSharedAccess>()
                }
            );
            await dbContext.SaveChangesAsync();

            var store = new EnvironmentsStore(dbContext);
            var envs = await store.GetAsync(EnvironmentAccessRight.Everything, new EnvironmentFilter());
            Assert.Single(envs);
        }

        [Fact]
        public async Task ShouldReturnEnvironmentWhenPurposeMatches()
        {
            var dbContext = InMemoryDbHelper.InitialiseDb<EnvironmentsDbContext>("Mocked", o => new EnvironmentsDbContext(o));
            dbContext.Add
            (
                new Environment
                {
                    Purpose = EnvironmentPurpose.Lucca,
                }
            );
            await dbContext.SaveChangesAsync();

            var store = new EnvironmentsStore(dbContext);
            var envs = await store.GetAsync(new List<EnvironmentAccessRight>
            {
                new EnvironmentAccessRight
                {
                    Purposes = PurposeAccessRight.ForSome(EnvironmentPurpose.Lucca),
                    AccessRight = AccessRight.All
                }
            }, new EnvironmentFilter());
            Assert.Single(envs);
        }

        [Fact]
        public async Task ShouldNotReturnEnvironmentWhenPurposeDoesNotMatch()
        {
            var dbContext = InMemoryDbHelper.InitialiseDb<EnvironmentsDbContext>("Mocked", o => new EnvironmentsDbContext(o));
            dbContext.Add
            (
                new Environment
                {
                    Purpose = EnvironmentPurpose.Lucca,
                }
            );
            await dbContext.SaveChangesAsync();

            var store = new EnvironmentsStore(dbContext);
            var envs = await store.GetAsync(new List<EnvironmentAccessRight>
            {
                new EnvironmentAccessRight
                {
                    Purposes = PurposeAccessRight.ForSome(EnvironmentPurpose.InternalTest),
                    AccessRight = AccessRight.All
                }
            }, new EnvironmentFilter());
            Assert.Empty(envs);
        }

        [Fact]
        public async Task ShouldReturnEnvironmentWhenDistributorMatches()
        {
            var dbContext = InMemoryDbHelper.InitialiseDb<EnvironmentsDbContext>("Mocked", o => new EnvironmentsDbContext(o));
            dbContext.Add
            (
                new Environment
                {
                    ActiveAccesses = new List<EnvironmentSharedAccess>
                    {
                        new EnvironmentSharedAccess { ConsumerId = "ApertureScience"}
                    }
                }
            );
            await dbContext.SaveChangesAsync();

            var store = new EnvironmentsStore(dbContext);
            var envs = await store.GetAsync(new List<EnvironmentAccessRight>
            {
                new EnvironmentAccessRight
                {
                    Purposes = PurposeAccessRight.ForAll,
                    AccessRight = AccessRight.ForDistributor("ApertureScience")
                }
            }, new EnvironmentFilter());
            Assert.Single(envs);
        }

        [Fact]
        public async Task ShouldNotReturnEnvironmentWhenDistributorDoesNotMatch()
        {
            var dbContext = InMemoryDbHelper.InitialiseDb<EnvironmentsDbContext>("Mocked", o => new EnvironmentsDbContext(o));
            dbContext.Add
            (
                new Environment
                {
                    ActiveAccesses = new List<EnvironmentSharedAccess>
                    {
                        new EnvironmentSharedAccess { ConsumerId = "ApertureScience"}
                    }
                }
            );
            await dbContext.SaveChangesAsync();

            var store = new EnvironmentsStore(dbContext);
            var envs = await store.GetAsync(new List<EnvironmentAccessRight>
            {
                new EnvironmentAccessRight
                {
                    Purposes = PurposeAccessRight.ForAll,
                    AccessRight = AccessRight.ForDistributor("BlackMesa")
                }
            }, new EnvironmentFilter());
            Assert.Empty(envs);
        }

        [Fact]
        public async Task ShouldReturnEnvironmentWhenOneAccessCompletelyMatches()
        {
            var dbContext = InMemoryDbHelper.InitialiseDb<EnvironmentsDbContext>("Mocked", o => new EnvironmentsDbContext(o));
            dbContext.Add
            (
                new Environment
                {
                    Purpose = EnvironmentPurpose.Lucca,
                    ActiveAccesses = new List<EnvironmentSharedAccess>
                    {
                        new EnvironmentSharedAccess { ConsumerId = "ApertureScience"}
                    }
                }
            );
            await dbContext.SaveChangesAsync();

            var store = new EnvironmentsStore(dbContext);
            var envs = await store.GetAsync(new List<EnvironmentAccessRight>
            {
                new EnvironmentAccessRight
                {
                    Purposes = PurposeAccessRight.ForSome(EnvironmentPurpose.Lucca),
                    AccessRight = AccessRight.ForDistributor("ApertureScience")
                },
            }, new EnvironmentFilter());
            Assert.Single(envs);
        }

        [Fact]
        public async Task ShouldNotReturnEnvironmentWhenAccessesPartiallyMatch()
        {
            var dbContext = InMemoryDbHelper.InitialiseDb<EnvironmentsDbContext>("Mocked", o => new EnvironmentsDbContext(o));
            dbContext.Add
            (
                new Environment
                {
                    Purpose = EnvironmentPurpose.Lucca,
                    ActiveAccesses = new List<EnvironmentSharedAccess>
                    {
                        new EnvironmentSharedAccess { ConsumerId = "ApertureScience"}
                    }
                }
            );
            await dbContext.SaveChangesAsync();

            var store = new EnvironmentsStore(dbContext);
            var envs = await store.GetAsync(new List<EnvironmentAccessRight>
            {
                new EnvironmentAccessRight
                {
                    Purposes = PurposeAccessRight.ForSome(EnvironmentPurpose.Lucca),
                    AccessRight = AccessRight.ForDistributor("BlackMesa")
                },
                new EnvironmentAccessRight
                {
                    Purposes = PurposeAccessRight.ForSome(EnvironmentPurpose.Contractual),
                    AccessRight = AccessRight.ForDistributor("ApertureScience")
                },
            }, new EnvironmentFilter());
            Assert.Empty(envs);
        }
    }
}
