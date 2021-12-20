using Distributors.Domain.Models;
using Environments.Domain;
using Environments.Domain.Storage;
using Environments.Infra.Storage;
using Environments.Infra.Storage.Stores;
using FluentAssertions;
using Lucca.Core.Api.Queryable.Paging;
using Lucca.Core.Shared.Domain.Exceptions;
using Moq;
using Moq.Protected;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Testing.Infra;
using Xunit;
using Environment = Environments.Domain.Environment;

namespace Environments.Infra.Tests
{
    public class EnvironmentStoreTests
    {
        private readonly Mock<IQueryPager> _queryPager;
        private readonly Mock<HttpMessageHandler> _mockMessageHandler;
        private readonly HttpClient _httpClient;

        public EnvironmentStoreTests()
        {
            _queryPager = new Mock<IQueryPager>(MockBehavior.Strict);
            _mockMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_mockMessageHandler.Object)
            {
                BaseAddress = new Uri("http://cc-legacy")
            };
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

            var store = new EnvironmentsStore(dbContext, _queryPager.Object, _httpClient);
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

            var store = new EnvironmentsStore(dbContext, _queryPager.Object, _httpClient);
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

            var store = new EnvironmentsStore(dbContext, _queryPager.Object, _httpClient);
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
                        SharedAccessForConsumer(777)
                    }
                }
            );
            await dbContext.SaveChangesAsync();

            var store = new EnvironmentsStore(dbContext, _queryPager.Object, _httpClient);
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
                        SharedAccessForConsumer(777)
                    }
                }
            );
            await dbContext.SaveChangesAsync();

            var store = new EnvironmentsStore(dbContext, _queryPager.Object, _httpClient);
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
                        SharedAccessForConsumer(777)
                    }
                }
            );
            await dbContext.SaveChangesAsync();

            var store = new EnvironmentsStore(dbContext, _queryPager.Object, _httpClient);
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
                        SharedAccessForConsumer(777)
                    }
                }
            );
            await dbContext.SaveChangesAsync();

            var store = new EnvironmentsStore(dbContext, _queryPager.Object, _httpClient);

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

        private static EnvironmentSharedAccess SharedAccessForConsumer(int consumerId) => new EnvironmentSharedAccess
            { ConsumerId = consumerId, Access = new EnvironmentAccess() };

        #region UpdateSubDomainAsync
        [Fact]
        public async Task UpdateSubDomainAsync_Ok()
        {
            var store = new EnvironmentsStore(null, _queryPager.Object, _httpClient);
            var environment = new Environment
            {
                Id = 42,
                Subdomain = "oldSubDomain"
            };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            };
            _mockMessageHandler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            await store.UpdateSubDomainAsync(environment, "newSubDomain");
        }

        [Fact]
        public async Task UpdateSubDomainAsync_BadRequest()
        {
            var store = new EnvironmentsStore(null, _queryPager.Object, _httpClient);
            var environment = new Environment
            {
                Id = 42,
                Subdomain = "oldSubDomain"
            };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("")
            };
            _mockMessageHandler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            Func<Task> act = () => store.UpdateSubDomainAsync(environment, "newSubDomain");

            (await act
                .Should().ThrowAsync<DomainException>())
                .And.Status.Should().Be(DomainExceptionCode.BadRequest);
        }

        [Fact]
        public async Task UpdateSubDomainAsync_Forbidden()
        {
            var store = new EnvironmentsStore(null, _queryPager.Object, _httpClient);
            var environment = new Environment
            {
                Id = 42,
                Subdomain = "oldSubDomain"
            };
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Forbidden,
                Content = new StringContent("")
            };
            _mockMessageHandler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            Func<Task> act = () => store.UpdateSubDomainAsync(environment, "newSubDomain");

            (await act
                .Should().ThrowAsync<DomainException>())
                .And.Status.Should().Be(DomainExceptionCode.InternalServerError);
        }



        #endregion
    }
}
