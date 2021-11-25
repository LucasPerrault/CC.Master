using Billing.Contracts.Domain.Offers;
using Billing.Contracts.Domain.Tests;
using Billing.Contracts.Infra.Storage;
using Billing.Contracts.Infra.Storage.Stores;
using Billing.Products.Domain;
using FluentAssertions;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Microsoft.EntityFrameworkCore;
using Moq;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Billing.Contracts.Infra.Tests
{
    public class CommercialOffersStoreTests
    {
        private readonly ContractsDbContext _dbContext;
        private readonly Mock<IQueryPager> _queryPagerMock;
        private readonly CommercialOffersStore _store;

        public CommercialOffersStoreTests()
        {
            var options = new DbContextOptionsBuilder<ContractsDbContext>()
                               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                               .Options;

            _dbContext = new ContractsDbContext(options);
            _queryPagerMock = new Mock<IQueryPager>();
            _queryPagerMock
                .Setup(p => p.ToPageAsync(It.IsAny<IQueryable<CommercialOffer>>(), It.IsAny<IPageToken>()))
                .Returns<IQueryable<CommercialOffer>, IPageToken>(
                    (queryable, pageToken) => Task.FromResult(new Page<CommercialOffer> { Items = queryable.ToList() })
                );
            _store = new CommercialOffersStore(_dbContext, _queryPagerMock.Object);

            _dbContext.Add(new Product { Id = 1, Name = "default" });
            _dbContext.Add(new Product { Id = 42, Name = "miaou" });
            _dbContext.Add(new Product { Id = 52, Name = "other" });
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task ShouldGetSimilarOffersAsync_ReturnEmptyWhen_SameOffer_Async()
        {
            var referenceOffer = new CommercialOffer().Build()
                .With(name: "miaou", tag: "miaou", productId: 42, currencyId: 42)
                .WithPriceList(startingOn: new DateTime(2021, 01, 01))
                .AndPriceRow(maxExcludedCount: 5, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 10, unitPrice: 1, fixedPrice: 1);

            await _dbContext.AddAsync(referenceOffer);
            await _dbContext.SaveChangesAsync();

            var similarOffers = await _store.GetSimilarOffersAsync(AccessRight.All, referenceOffer, It.IsAny<DateTime>());

            similarOffers.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task ShouldGetSimilarOffersAsync_ReturnOfferWhen_FilteredPropertiesAreIdentical_Async()
        {
            var referenceOffer = new CommercialOffer().Build()
                .With(name: "miaou1", tag: "miaou1", productId: 42, currencyId: 42)
                .WithPriceList(startingOn: new DateTime(2021, 01, 01))
                .AndPriceRow(maxExcludedCount: 5, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 10, unitPrice: 1, fixedPrice: 1);
            var targetOffer = new CommercialOffer().Build()
                .With(name: "miaou2", tag: "miaou2", productId: 42, currencyId: 42)
                .WithPriceList(startingOn: new DateTime(2021, 01, 01))
                .AndPriceRow(maxExcludedCount: 5, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 10, unitPrice: 1, fixedPrice: 1);
            var otherOffer = new CommercialOffer().Build()
                .With(productId: 1)
                .WithPriceList(startingOn: new DateTime(2021, 01, 01))
                .AndPriceRow(maxExcludedCount: 5, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 10, unitPrice: 1, fixedPrice: 1);

            await _dbContext.AddAsync(referenceOffer);
            await _dbContext.AddAsync(targetOffer);
            await _dbContext.AddAsync(otherOffer);
            await _dbContext.SaveChangesAsync();

            var similarOffers = await _store.GetSimilarOffersAsync(AccessRight.All, referenceOffer, new DateTime(2022, 01, 01));

            similarOffers.Items.Should().ContainSingle(o => o.Id == targetOffer.Id);
        }

        public static IEnumerable<object[]> DifferentPropertyTestData()
        {
            yield return new object[] { new CommercialOffer().Build().With(productId: 52, currencyId: 42) };
            yield return new object[] { new CommercialOffer().Build().With(productId: 42, currencyId: 52) };
            yield return new object[] { new CommercialOffer().Build().With(productId: 42, currencyId: 42, billingMode: BillingMode.UsersWithAccess) };
            yield return new object[] { new CommercialOffer().Build().With(productId: 42, currencyId: 42, pricingMethod: PricingMethod.AnnualCommitment) };
            yield return new object[] { new CommercialOffer().Build().With(productId: 42, currencyId: 42, forecastMethod: ForecastMethod.AnnualCommitment) };
        }
        [Theory]
        [MemberData(nameof(DifferentPropertyTestData))]
        public async Task ShouldGetSimilarOffersAsync_ReturnEmptyWhen_AnyFilteredPropertyIsDifferent_Async(CommercialOffer targetOffer)
        {
            var referenceOffer = new CommercialOffer().Build()
                .With(productId: 42, currencyId: 42, billingMode: BillingMode.AllUsers, pricingMethod: PricingMethod.Constant, forecastMethod: ForecastMethod.LastRealMonth);

            await _dbContext.AddAsync(referenceOffer);
            await _dbContext.AddAsync(targetOffer);
            await _dbContext.SaveChangesAsync();

            var similarOffers = await _store.GetSimilarOffersAsync(AccessRight.All, referenceOffer, new DateTime(2022, 01, 01));

            similarOffers.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task ShouldGetSimilarOffersAsync_ReturnEmptyWhen_AnyOfferPriceListUntil_HasLessPriceRows_Async()
        {
            var referenceOffer = new CommercialOffer().Build()
                .With(productId: 1)
                .WithPriceList(startingOn: new DateTime(2021, 01, 01))
                .AndPriceRow(maxExcludedCount: 5, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 10, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 20, unitPrice: 1, fixedPrice: 1);
            var targetOffer = new CommercialOffer().Build()
                .With(productId: 1)
                .WithPriceList(startingOn: new DateTime(2021, 01, 01))
                .AndPriceRow(maxExcludedCount: 5, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 10, unitPrice: 1, fixedPrice: 1);

            await _dbContext.AddAsync(referenceOffer);
            await _dbContext.AddAsync(targetOffer);
            await _dbContext.SaveChangesAsync();

            var similarOffers = await _store.GetSimilarOffersAsync(AccessRight.All, referenceOffer, new DateTime(2022, 01, 01));

            similarOffers.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task ShouldGetSimilarOffersAsync_ReturnOfferWhen_AllPriceListsUntil_HaveIdenticalStartsOnAndRows_Async()
        {
            var referenceOffer = new CommercialOffer().Build()
                .With(productId: 1)
                .WithPriceList(startingOn: new DateTime(2021, 01, 01))
                .AndPriceRow(maxExcludedCount: 5, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 10, unitPrice: 1, fixedPrice: 1);
            var targetOffer = new CommercialOffer().Build()
                .With(productId: 1)
                .WithPriceList(startingOn: new DateTime(2021, 01, 01))
                .AndPriceRow(maxExcludedCount: 5, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 10, unitPrice: 1, fixedPrice: 1);

            await _dbContext.AddAsync(referenceOffer);
            await _dbContext.AddAsync(targetOffer);
            await _dbContext.SaveChangesAsync();

            var similarOffers = await _store.GetSimilarOffersAsync(AccessRight.All, referenceOffer, new DateTime(2022, 01, 01));

            similarOffers.Items.Should().ContainSingle(o => o.Id == targetOffer.Id);
        }

        [Fact]
        public async Task ShouldGetSimilarOffersAsync_ReturnOfferWhen_PriceListsAfter_HaveLessPriceRows_Async()
        {
            var referenceOffer = new CommercialOffer().Build()
                .With(productId: 1)
                .WithPriceList(startingOn: new DateTime(2021, 01, 01))
                .AndPriceRow(maxExcludedCount: 5, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 10, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 20, unitPrice: 1, fixedPrice: 1);
            var targetOffer = new CommercialOffer().Build()
                .With(productId: 1)
                .WithPriceList(startingOn: new DateTime(2021, 01, 01))
                .AndPriceRow(maxExcludedCount: 5, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 10, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 20, unitPrice: 1, fixedPrice: 1)
                .WithPriceList(startingOn: new DateTime(2023, 01, 01))
                .AndPriceRow(maxExcludedCount: 5, unitPrice: 1, fixedPrice: 1);

            await _dbContext.AddAsync(referenceOffer);
            await _dbContext.AddAsync(targetOffer);
            await _dbContext.SaveChangesAsync();

            var similarOffers = await _store.GetSimilarOffersAsync(AccessRight.All, referenceOffer, new DateTime(2022, 01, 01));

            similarOffers.Items.Should().ContainSingle(o => o.Id == targetOffer.Id);
        }

        [Fact]
        public async Task ShouldGetSimilarOffersAsync_ReturnEmptyWhen_AnyPriceListUntil_HaveDifferentStartsOn_Async()
        {
            var referenceOffer = new CommercialOffer().Build()
                .With(productId: 1)
                .WithPriceList(startingOn: new DateTime(2021, 01, 01))
                .AndPriceRow(maxExcludedCount: 5, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 10, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 20, unitPrice: 1, fixedPrice: 1);
            var targetOffer = new CommercialOffer().Build()
                .With(productId: 1)
                .WithPriceList(startingOn: new DateTime(2021, 11, 01))
                .AndPriceRow(maxExcludedCount: 5, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 10, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 20, unitPrice: 1, fixedPrice: 1);

            await _dbContext.AddAsync(referenceOffer);
            await _dbContext.AddAsync(targetOffer);
            await _dbContext.SaveChangesAsync();

            var similarOffers = await _store.GetSimilarOffersAsync(AccessRight.All, referenceOffer, new DateTime(2022, 01, 01));

            similarOffers.Items.Should().BeEmpty();
        }

        public static IEnumerable<object[]> DifferentRowTestData()
        {
            yield return new object[] { new CommercialOffer().Build()
                .With(productId: 1)
                .WithPriceList(startingOn: new DateTime(2021, 01, 01))
                .AndPriceRow(maxExcludedCount: 5, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 42, unitPrice: 1, fixedPrice: 1)
            };
            yield return new object[] { new CommercialOffer().Build()
                .With(productId: 1)
                .WithPriceList(startingOn: new DateTime(2021, 01, 01))
                .AndPriceRow(maxExcludedCount: 5, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 10, unitPrice: 42, fixedPrice: 1)
            };
            yield return new object[] { new CommercialOffer().Build()
                .With(productId: 1)
                .WithPriceList(startingOn: new DateTime(2021, 01, 01))
                .AndPriceRow(maxExcludedCount: 5, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 10, unitPrice: 1, fixedPrice: 42)
            };
        }
        [Theory]
        [MemberData(nameof(DifferentRowTestData))]
        public async Task ShouldGetSimilarOffersAsync_ReturnEmptyWhen_AnyPriceListUntil_HaveSameStartsOnButDifferentRow_Async(CommercialOffer targetOffer)
        {
            var referenceOffer = new CommercialOffer().Build()
                .With(productId: 1)
                .WithPriceList(startingOn: new DateTime(2021, 01, 01))
                .AndPriceRow(maxExcludedCount: 5, unitPrice: 1, fixedPrice: 1)
                .AndPriceRow(maxExcludedCount: 10, unitPrice: 1, fixedPrice: 1);

            await _dbContext.AddAsync(referenceOffer);
            await _dbContext.AddAsync(targetOffer);
            await _dbContext.SaveChangesAsync();

            var similarOffers = await _store.GetSimilarOffersAsync(AccessRight.All, referenceOffer, new DateTime(2022, 01, 01));

            similarOffers.Items.Should().BeEmpty();
        }
    }
}
