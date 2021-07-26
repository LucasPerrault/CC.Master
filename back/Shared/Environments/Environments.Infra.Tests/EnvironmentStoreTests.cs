using Distributors.Domain.Models;
using Environments.Domain;
using Environments.Domain.Storage;
using Environments.Infra.Storage;
using Environments.Infra.Storage.Stores;
using Lucca.Core.Api.Queryable.Paging;
using Moq;
using Rights.Domain.Filtering;
using System.Collections.Generic;
using System.Threading.Tasks;
using Testing.Infra;
using Xunit;

namespace Environments.Infra.Tests
{
    public class EnvironmentStoreTests
    {
        private readonly Mock<IQueryPager> _queryPager;

        public EnvironmentStoreTests()
        {
            _queryPager = new Mock<IQueryPager>();
        }

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

            var store = new EnvironmentsStore(dbContext, _queryPager.Object);
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

            var store = new EnvironmentsStore(dbContext, _queryPager.Object);
            var envs = await store.GetAsync(new List<EnvironmentAccessRight>
            {
                new EnvironmentAccessRight
                (
                    AccessRight.All,
                    PurposeAccessRight.ForSome(EnvironmentPurpose.Lucca)
                )
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

            var store = new EnvironmentsStore(dbContext, _queryPager.Object);
            var envs = await store.GetAsync(new List<EnvironmentAccessRight>
            {
                new EnvironmentAccessRight
                (
                    AccessRight.All,
                    PurposeAccessRight.ForSome(EnvironmentPurpose.InternalTest)
                )
            }, new EnvironmentFilter());
            Assert.Empty(envs);
        }

        [Fact]
        public async Task ShouldReturnEnvironmentWhenDistributorMatches()
        {
            var dbContext = InMemoryDbHelper.InitialiseDb<EnvironmentsDbContext>("Mocked", o => new EnvironmentsDbContext(o));
            dbContext.Add(new Distributor { Id = 777 });
            dbContext.Add
            (
                new Environment
                {
                    ActiveAccesses = new List<EnvironmentSharedAccess>
                    {
                        new EnvironmentSharedAccess { ConsumerId = 777 }
                    }
                }
            );
            await dbContext.SaveChangesAsync();

            var store = new EnvironmentsStore(dbContext, _queryPager.Object);
            var envs = await store.GetAsync(new List<EnvironmentAccessRight>
            {
                new EnvironmentAccessRight
                (
                    AccessRight.ForDistributor(777),
                    PurposeAccessRight.ForAll
                )
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
                        new EnvironmentSharedAccess { ConsumerId = 777}
                    }
                }
            );
            await dbContext.SaveChangesAsync();

            var store = new EnvironmentsStore(dbContext, _queryPager.Object);
            var envs = await store.GetAsync(new List<EnvironmentAccessRight>
            {
                new EnvironmentAccessRight
                (
                    AccessRight.ForDistributor(333),
                    PurposeAccessRight.ForAll
                )
            }, new EnvironmentFilter());
            Assert.Empty(envs);
        }

        [Fact]
        public async Task ShouldReturnEnvironmentWhenOneAccessCompletelyMatches()
        {
            var dbContext = InMemoryDbHelper.InitialiseDb<EnvironmentsDbContext>("Mocked", o => new EnvironmentsDbContext(o));
            dbContext.Add(new Distributor { Id = 777 });
            dbContext.Add
            (
                new Environment
                {
                    Purpose = EnvironmentPurpose.Lucca,
                    ActiveAccesses = new List<EnvironmentSharedAccess>
                    {
                        new EnvironmentSharedAccess { ConsumerId = 777 }
                    }
                }
            );
            await dbContext.SaveChangesAsync();

            var store = new EnvironmentsStore(dbContext, _queryPager.Object);
            var envs = await store.GetAsync(new List<EnvironmentAccessRight>
            {
                new EnvironmentAccessRight
                (
                    AccessRight.ForDistributor(777),
                    PurposeAccessRight.ForSome(EnvironmentPurpose.Lucca)
                )
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
                        new EnvironmentSharedAccess { ConsumerId = 777}
                    }
                }
            );
            await dbContext.SaveChangesAsync();

            var store = new EnvironmentsStore(dbContext, _queryPager.Object);

            var envs = await store.GetAsync(new List<EnvironmentAccessRight>
            {
                new EnvironmentAccessRight
                (
                    AccessRight.ForDistributor(333),
                    PurposeAccessRight.ForSome(EnvironmentPurpose.Lucca)
                ),
                new EnvironmentAccessRight
                (
                    AccessRight.ForDistributor(777),
                    PurposeAccessRight.ForSome(EnvironmentPurpose.Contractual)
                ),
            }, new EnvironmentFilter());
            Assert.Empty(envs);
        }
    }
}
