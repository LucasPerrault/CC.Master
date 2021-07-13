using Billing.Cmrr.Domain;
using Billing.Cmrr.Infra.Storage;
using Billing.Cmrr.Infra.Storage.Stores;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Billing.Cmrr.Infra.Tests
{
    public class CmrrCountsStoreTests
    {
        private readonly CmrrDbContext _dbContext;

        public CmrrCountsStoreTests()
        {
            var options = new DbContextOptionsBuilder<CmrrDbContext>()
                               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                               .Options;

            _dbContext = new CmrrDbContext(options);
        }

        [Fact]
        public async Task ShouldReturnCountsWithSameSpecifiedPeriodAsync()
        {
            var countPeriod = new DateTime(2021, 01, 01);

            var cmrrContracts = new List<CmrrCount>
            {
                new CmrrCount
                {
                    Id = 1,
                    CountPeriod = countPeriod
                },
                new CmrrCount
                {
                    Id = 2,
                    CountPeriod = countPeriod.AddMonths(-1)
                },
                new CmrrCount
                {
                    Id = 3,
                    CountPeriod = countPeriod.AddMonths(1)
                },
            };

            await _dbContext.AddRangeAsync(cmrrContracts);
            await _dbContext.SaveChangesAsync();

            var expectedIds = new List<int> { 1 };

            var sut = new CmrrCountsStore(_dbContext);
            var contracts = await sut.GetByPeriodsAsync(countPeriod);

            contracts.Should().NotBeNullOrEmpty();
            contracts.Should().HaveCount(1);
            contracts.Select(c => c.Id).Should().Contain(expectedIds);
        }
    }
}
