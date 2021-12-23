using Billing.Cmrr.Domain;
using Billing.Cmrr.Infra.Storage;
using Billing.Cmrr.Infra.Storage.Stores;
using FluentAssertions;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testing.Infra;
using Xunit;

namespace Billing.Cmrr.Infra.Tests
{
    public class CmrrContractsStoreTests
    {
        private readonly CmrrDbContext _dbContext;

        public CmrrContractsStoreTests()
        {
            _dbContext = InMemoryDbHelper.InitialiseDb<CmrrDbContext>("cmrr", o => new CmrrDbContext(o));
        }

        [Fact]
        public async Task ShouldReturnContractsNotEndedAsync()
        {
            var startCountPeriod = new DateTime(2021, 01, 01);
            var endCountPeriod = new DateTime(2021, 02, 01);

            var cmrrContracts = new List<CmrrContract>
            {
                new CmrrContract
                {
                    Id = 1,
                    StartDate = startCountPeriod,
                    IsArchived = false
                },
                new CmrrContract
                {
                    Id = 2,
                    StartDate = startCountPeriod,
                    EndDate = endCountPeriod,
                    IsArchived = false
                },
                new CmrrContract
                {
                    Id = 3,
                    StartDate = startCountPeriod.AddMonths(-2),
                    EndDate = startCountPeriod.AddMonths(-1),
                    IsArchived = false
                },
                new CmrrContract
                {
                    Id = 4,
                    StartDate = endCountPeriod.AddMonths(1),
                    EndDate = endCountPeriod.AddMonths(2),
                    IsArchived = false
                }
            };

            await _dbContext.AddRangeAsync(cmrrContracts);
            await _dbContext.SaveChangesAsync();

            var expectedIds = new List<int> { 1, 2 };

            var sut = new CmrrContractsStore(_dbContext);
            var contracts = await sut.GetContractsNotEndedAtAsync(startCountPeriod, endCountPeriod, AccessRight.All);

            contracts.Should().NotBeNullOrEmpty();
            contracts.Should().HaveCount(2);
            contracts.Select(c => c.Id).Should().Contain(expectedIds);
        }

        [Fact]
        public async Task ShouldReturnContractsNotArchivedAsync()
        {
            var startCountPeriod = new DateTime(2021, 01, 01);
            var endCountPeriod = new DateTime(2021, 02, 01);

            var cmrrContracts = new List<CmrrContract>
            {
                new CmrrContract
                {
                    Id = 1,
                    StartDate = startCountPeriod,
                    IsArchived = false
                },
                new CmrrContract
                {
                    Id = 2,
                    StartDate = startCountPeriod,
                    EndDate = endCountPeriod,
                    IsArchived = true
                }
            };

            await _dbContext.AddRangeAsync(cmrrContracts);
            await _dbContext.SaveChangesAsync();

            var expectedIds = new List<int> { 1 };

            var sut = new CmrrContractsStore(_dbContext);
            var contracts = await sut.GetContractsNotEndedAtAsync(startCountPeriod, endCountPeriod, AccessRight.All);

            contracts.Should().NotBeNullOrEmpty();
            contracts.Should().HaveCount(1);
            contracts.Select(c => c.Id).Should().Contain(expectedIds);
        }

    }
}
