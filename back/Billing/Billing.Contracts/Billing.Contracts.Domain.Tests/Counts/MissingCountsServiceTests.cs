using System;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Authentication.Domain;
using Billing.Contracts.Domain.Clients;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Counts;
using Billing.Contracts.Domain.Counts.Services;
using Billing.Contracts.Domain.Environments;
using Billing.Contracts.Domain.Offers;
using Billing.Contracts.Infra.Storage;
using Billing.Contracts.Infra.Storage.Stores;
using Billing.Products.Domain;
using Distributors.Domain.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Moq;
using NExtends.Primitives.DateTimes;
using Rights.Domain.Abstractions;
using Rights.Domain.Filtering;
using Testing.Infra;
using Users.Domain;
using Xunit;

namespace Billing.Contracts.Domain.Tests.Counts
{
    public class MissingCountsServiceTests
    {
        private readonly IMissingCountsService _missingCountsService;
        private readonly ContractsDbContext _contractsDbContext;

        public MissingCountsServiceTests()
        {
            var queryPagerMock = new Mock<IQueryPager>();
            queryPagerMock
                .Setup(p => p.ToPageAsync(It.IsAny<IQueryable<Contract>>(), It.IsAny<IPageToken>()))
                .Returns<IQueryable<Contract>, IPageToken>(
                    (queryable, pageToken) => Task.FromResult(new Page<Contract> { Items = queryable.ToList() })
                );

            _contractsDbContext = InMemoryDbHelper.InitialiseDb<ContractsDbContext>("Contracts", o => new ContractsDbContext(o));
            var contractsStore = new ContractsStore(_contractsDbContext, queryPagerMock.Object);
            var contractsRightsFilter = new ContractsRightsFilter(new RightsFilter(new Mock<IRightsService>().Object));

            var principal = new Principal
                {UserId = 1, User = new User {Id = 1, Distributor = new Distributor {Id = 37}}};

            _missingCountsService = new MissingCountsService(contractsStore, contractsRightsFilter, new CloudControlUserClaimsPrincipal(principal));
        }

        [Fact]
        public async Task ShouldGet_MissingCountsAsync()
        {
            var period = new DateTime(2021, 12, 01);

            var environment = new ContractEnvironment {Id = 1};
            var contractWithCount = new Contract
            {
                Id = 1,
                TheoreticalEndOn = period.AddMonths(1).LastOfMonth(),
                Environment = environment,
                ArchivedAt = null,
                Attachments = new List<EstablishmentAttachment> { new()
                {
                    Id = 1, StartsOn = period.AddMonths(-10).FirstOfMonth(), EstablishmentId = 1, EstablishmentName = "fake-ets1"
                }},
                Client = new Client { Id = 1 },
                Distributor = new Distributor { Id = 37 },
                CommercialOffer = new CommercialOffer { Id = 1, Name = "fake-offer1", Product = new Product { Id = 1 }}
            };
            var contractWithoutCount = new Contract
            {
                Id = 2,
                TheoreticalEndOn = period.AddMonths(1).LastOfMonth(),
                Environment = environment,
                ArchivedAt = null,
                Attachments = new List<EstablishmentAttachment> { new()
                {
                    Id = 2, StartsOn = period.AddMonths(-10).FirstOfMonth(), EstablishmentId = 2, EstablishmentName = "fake-ets2"
                }},
                Client = new Client { Id = 2 },
                Distributor = new Distributor { Id = 36 },
                CommercialOffer = new CommercialOffer { Id = 2, Name = "fake-offer2", Product = new Product { Id = 2 }}
            };
            await _contractsDbContext.AddAsync(contractWithCount);
            await _contractsDbContext.AddAsync(contractWithoutCount);
            await _contractsDbContext.SaveChangesAsync();

            var countsOverPeriod = new List<Count> { new() {Id = 1, ContractId = contractWithCount.Id, CountPeriod = period} };
            var missingCounts = await _missingCountsService.GetAsync(countsOverPeriod, period);

            missingCounts.Single().ContractId.Should().Be(contractWithoutCount.Id);
        }
    }
}
