using Billing.Contracts.Domain.Offers;
using Billing.Contracts.Domain.Offers.Validation;
using Billing.Contracts.Domain.Offers.Validation.Exceptions;
using FluentAssertions;
using Moq;
using Resources.Translations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tools;
using Xunit;

namespace Billing.Contracts.Domain.Tests
{
    public class CommercialOfferValidationTests
    {
        private readonly Mock<ITimeProvider> _time;
        private readonly Mock<IContractsTranslations> _translations;

        private CommercialOfferValidationService Validation => new CommercialOfferValidationService(_time.Object, _translations.Object);

        public CommercialOfferValidationTests()
        {
            _time = new Mock<ITimeProvider>();
            _translations = new Mock<IContractsTranslations>();
        }

        [Fact]
        public void CreateOffer_Validation_ShouldThrowOnlyIf_PriceListStartDate_IsNotFirstDayOfTheMonth()
        {
            var offer = new CommercialOffer().Build()
                .WithPriceList(startingOn: new DateTime(2020, 01, 02));
            ShouldThrowWhenCreate(offer, t => t.PriceListStartsOnFirstOfMonth());

            Reset();

            var offer2 = new CommercialOffer().Build()
                .WithPriceList(startingOn: new DateTime(2020, 01, 01));
            ShouldNotThrowWhenCreate(offer2);
        }

        [Fact]
        public void AddPriceList_Validation_ShouldThrowOnlyIf_PriceListStartDate_IsNotFirstDayOfTheMonth()
        {
            var offer = new CommercialOffer().Build();

            var priceList = new PriceList().CreateFor(offer)
                .StartingOn(new DateTime(2020, 01, 02));
            ShouldThrowWhenAdd(priceList, offer, t => t.PriceListStartsOnFirstOfMonth());

            Reset();

            priceList
                .StartingOn(new DateTime(2020, 01, 01));
            ShouldNotThrowWhenAdd(priceList, offer);
        }

        [Fact]
        public void ModifyPriceList_Validation_ShouldThrowOnlyIf_PriceListStartDate_IsNotFirstDayOfTheMonth()
        {
            var offer = new CommercialOffer().Build()
                .WithPriceList()
                .WithPriceList(startingOn: new DateTime(2020, 01, 01));
            var oldPriceList = offer.PriceLists.Last();
            var usage = new CommercialOfferUsage();

            var newPriceList = new PriceList().BuildFor(offer, offer.PriceLists.First().Id)
                .StartingOn(new DateTime(2020, 01, 02));
            ShouldThrowWhenModify(oldPriceList, newPriceList, offer, usage, t => t.PriceListStartsOnFirstOfMonth());

            Reset();

            newPriceList
                .StartingOn(new DateTime(2021, 01, 01));
            ShouldNotThrowWhenModify(oldPriceList, newPriceList, offer, usage);
        }

        [Fact]
        public void AddPriceList_Validation_ShouldThrowOnlyIf_PriceListRows_AreNotOrdered()
        {
            var offer = new CommercialOffer()
                .Build()
                .WithPriceList(new DateTime(2002, 01, 01));

            var updatedList = new PriceList().BuildFor(offer)
                .StartingOn(new DateTime(2003, 01, 01))
                .WithPriceRow(maxIncludedCount: 10, fixedPrice: 1, unitPrice: 0)
                .WithPriceRow(maxIncludedCount: 25, fixedPrice: 1, unitPrice: 0)
                .WithPriceRow(maxIncludedCount: 30, fixedPrice: 1, unitPrice: 0);
            ShouldNotThrowWhenAdd(updatedList, offer);

             var unorderedList = new PriceList().BuildFor(offer)
                 .StartingOn(new DateTime(2004, 01, 01))
                 .WithPriceRow(maxIncludedCount: 10, fixedPrice: 1, unitPrice: 0)
                 .WithPriceRow(maxIncludedCount: 30, fixedPrice: 1, unitPrice: 0)
                 .WithPriceRow(maxIncludedCount: 25, fixedPrice: 1, unitPrice: 0);
             ShouldThrowWhenAdd(unorderedList, offer, t => t.PriceRowsNotOrdered());
        }

        [Fact]
        public void ModifyPriceList_Validation_ShouldThrowOnlyIf_PriceListRows_AreNotOrdered()
        {
            var usage = new CommercialOfferUsage { OfferId = 1 };
            var offer = new CommercialOffer()
                .Build()
                .WithPriceList(new DateTime(2002, 01, 01))
                .AndPriceRow(maxIncludedCount: 10, fixedPrice: 1, unitPrice: 0)
                .AndPriceRow(maxIncludedCount: 20, fixedPrice: 1, unitPrice: 0)
                .AndPriceRow(maxIncludedCount: 30, fixedPrice: 1, unitPrice: 0);

            var updatedList = new PriceList().BuildFor(offer, offer.PriceLists.First().Id)
                .StartingOn(new DateTime(2002, 01, 01))
                .WithPriceRow(maxIncludedCount: 10, fixedPrice: 1, unitPrice: 0)
                .WithPriceRow(maxIncludedCount: 25, fixedPrice: 1, unitPrice: 0)
                .WithPriceRow(maxIncludedCount: 30, fixedPrice: 1, unitPrice: 0);
            ShouldNotThrowWhenModify(offer.PriceLists.First(), updatedList, offer, usage);

            var unorderedList = new PriceList().BuildFor(offer, offer.PriceLists.First().Id)
                .StartingOn(new DateTime(2002, 01, 01))
                .WithPriceRow(maxIncludedCount: 10, fixedPrice: 1, unitPrice: 0)
                .WithPriceRow(maxIncludedCount: 30, fixedPrice: 1, unitPrice: 0)
                .WithPriceRow(maxIncludedCount: 25, fixedPrice: 1, unitPrice: 0);
            ShouldThrowWhenModify(offer.PriceLists.First(), unorderedList, offer, usage, t => t.PriceRowsNotOrdered());
        }

        [Fact]
        public void CreateOffer_Validation_ShouldThrowIf_PriceLists_HaveSame_StartDate()
        {
            var startDate = new DateTime(2020, 01, 01);

            var offer = new CommercialOffer().Build()
                .WithPriceList(startDate)
                .WithPriceList(startDate);
            ShouldThrowWhenCreate(offer, t => t.PriceListsStartsOnSameDay());

            Reset();

            offer.PriceLists.Last()
                .StartingOn(new DateTime(2021, 01, 01));
            ShouldNotThrowWhenCreate(offer);
        }

        [Fact]
        public void AddPriceList_Validation_ShouldThrowIf_PriceLists_HaveSameStartDate()
        {
            var startDate = new DateTime(2020, 01, 01);
            var offer = new CommercialOffer().Build()
                .WithPriceList(startingOn: startDate);

            var newPriceList = new PriceList().CreateFor(offer)
                .StartingOn(startDate);
            ShouldThrowWhenAdd(newPriceList, offer, t => t.PriceListsStartsOnSameDay());

            Reset();

            newPriceList
                .StartingOn(new DateTime(2021, 01, 01));
            ShouldNotThrowWhenAdd(newPriceList, offer);
        }

        [Fact]
        public void ModifyPriceList_Validation_ShouldThrowOnlyIf_PriceLists_HaveSameStartDate()
        {
            var startDate = new DateTime(2020, 01, 01);
            var offer = new CommercialOffer().Build()
                .WithPriceList()
                .WithPriceList(startingOn: startDate)
                .WithPriceList(startingOn: new DateTime(2022, 01, 01));
            var usage = new CommercialOfferUsage();
            var targetPriceList = offer.PriceLists.Last();

            var newPriceList = new PriceList().BuildFor(offer, targetPriceList.Id)
                .StartingOn(startDate);
            ShouldThrowWhenModify(targetPriceList, newPriceList, offer, usage, t => t.PriceListsStartsOnSameDay());

            Reset();

            newPriceList
                .StartingOn(startDate.AddMonths(1));
            ShouldNotThrowWhenModify(targetPriceList, newPriceList, offer, usage);
        }

        [Fact]
        public void ModifyOffer_Validation_ShouldThrowWhen_PayloadPriceListIsNotNull()
        {
            var oldOffer = new CommercialOffer().Build()
                .WithPriceList();
            var payload = new CommercialOffer().Build(oldOffer.Id)
                .WithPriceList();
            var usageWithoutCount = new CommercialOfferUsage().BuildFor(oldOffer);

            ShouldThrowWhenModify(oldOffer, payload, usageWithoutCount, t => t.PriceListChanged());

            Reset();

            payload.PriceLists = null;
            ShouldNotThrowWhenModify(oldOffer, payload, usageWithoutCount);
        }

        [Fact]
        public void ModifyOffer_Validation_ShouldNotThrowWhen_NameOrTagChanged_EvenWithCount()
        {
            var oldOffer = new CommercialOffer().Build()
                .With(name: "miaou", tag: "miaou", isArchived: false);
            var newOffer = new CommercialOffer().Build(oldOffer.Id)
                .With(name: "forty two", tag: "miaou", isArchived: false);
            var usageWithCount = new CommercialOfferUsage().BuildFor(oldOffer)
                .WithCountedContractsNumber(1);

            ShouldNotThrowWhenModify(oldOffer, newOffer, usageWithCount);

            Reset();

            newOffer.Name = "miaou";
            newOffer.Tag = "forty two";
            newOffer.IsArchived = false;
            ShouldNotThrowWhenModify(oldOffer, newOffer, usageWithCount);

            Reset();

            newOffer.Name = "miaou";
            newOffer.Tag = "miaou";
            newOffer.IsArchived = true;
            ShouldNotThrowWhenModify(oldOffer, newOffer, usageWithCount);
        }

        [Fact]
        public void ModifyOffer_Validation_ShouldThrowIf_PropertiesChanged_DespiteCount()
        {
            var oldOffer = new CommercialOffer().Build()
                .With
                (
                    name: "miaou",
                    tag: "miaou",
                    billingMode: BillingMode.AllUsers
                );
            var newOffer = new CommercialOffer().Build(oldOffer.Id)
                .With
                (
                    name: "forty two",
                    tag: "forty two",
                    billingMode: BillingMode.FlatFee
                );

            var usageWithCount = new CommercialOfferUsage().BuildFor(oldOffer)
                .WithCountedContractsNumber(1);
            ShouldThrowWhenModify(oldOffer, newOffer, usageWithCount, t => t.OfferChangedDespiteCount());

            Reset();

            var usageWithoutCount = new CommercialOfferUsage().BuildFor(oldOffer)
                .WithCountedContractsNumber(0);
            ShouldNotThrowWhenModify(oldOffer, newOffer, usageWithoutCount);
        }

        [Fact]
        public void ModifyPriceList_Validation_ShouldThrowWhen_PayloadPriceRowsIsNull()
        {
            var offer = new CommercialOffer().Build()
                .WithPriceList()
                .WithPriceList(startingOn: new DateTime(2020, 01, 01));
            var oldPriceList = offer.PriceLists.Last();
            var usageWithoutCount = new CommercialOfferUsage().BuildFor(offer);

            var payload = new PriceList().BuildFor(offer, oldPriceList.Id)
                .StartingOn(new DateTime(2020, 01, 01))
                .WithoutPriceRow();
            ShouldThrowWhenModify(oldPriceList, payload, offer, usageWithoutCount, t => t.PriceListShouldHaveRows());

            Reset();

            payload.WithNewPriceRow(maxIncludedCount:10);
            ShouldNotThrowWhenModify(oldPriceList, payload, offer, usageWithoutCount);
        }

        [Fact]
        public void ModifyPriceList_Validation_ShouldThrowOnlyIf_FutureStartDateChanged_HavingACount()
        {
            var offer = new CommercialOffer().Build()
                .WithPriceList(new DateTime(2000, 01, 01))
                .WithPriceList(new DateTime(2050, 01, 01));
            var oldPriceList = offer.PriceLists.Last();
            var newPriceList = new PriceList().BuildFor(offer, oldPriceList.Id)
                .StartingOn(new DateTime(2050, 02, 01));

            var usageWithCount = new CommercialOfferUsage().BuildFor(offer)
                .WithCountedContractsNumber(1);
            ShouldThrowWhenModify(oldPriceList, newPriceList, offer, usageWithCount, t => t.PriceListChangedDespiteCount());

            Reset();

            var usageWithoutCount = new CommercialOfferUsage().BuildFor(offer)
                .WithCountedContractsNumber(0);
            ShouldNotThrowWhenModify(oldPriceList, newPriceList, offer, usageWithoutCount);
        }

        [Fact]
        public void ModifyPriceList_Validation_ShouldThrowOnlyIf_RowChanged_HavingACount()
        {
            var offer = new CommercialOffer().Build()
                .WithPriceList()
                .AndPriceRow(maxIncludedCount: 42);
            var oldPriceList = offer.PriceLists.First();
            var newPriceList = new PriceList().BuildFor(offer, oldPriceList.Id)
                .WithPriceRow
                (
                    id: oldPriceList.Rows.First().Id,
                    maxIncludedCount: 24
                );

            var usageWithCount = new CommercialOfferUsage().BuildFor(offer)
                .WithCountedContractsNumber(1);
            ShouldThrowWhenModify(oldPriceList, newPriceList, offer, usageWithCount, t => t.PriceListChangedDespiteCount());

            Reset();

            var usageWithoutCount = new CommercialOfferUsage().BuildFor(offer)
                .WithCountedContractsNumber(0);
            ShouldNotThrowWhenModify(oldPriceList, newPriceList, offer, usageWithoutCount);
        }

        [Fact]
        public void ModifyPriceList_Validation_ShouldOnlyThrowIf_RowAdded_IsNotOnTop_WhenHavingACount()
        {
            var offer = new CommercialOffer().Build()
                .WithPriceList()
                .AndPriceRow(maxIncludedCount: 42);
            var oldPriceList = offer.PriceLists.First();
            var usageWithCount = new CommercialOfferUsage().BuildFor(offer)
                .WithCountedContractsNumber(1);

            var newPriceList = new PriceList().BuildFor(offer, oldPriceList.Id)
                .WithPriceRow(id: oldPriceList.Rows.First().Id, maxIncludedCount: 42)
                .WithNewPriceRow(maxIncludedCount: 9001);
            ShouldNotThrowWhenModify(oldPriceList, newPriceList, offer, usageWithCount);

            Reset();

            var newPriceList2 = new PriceList().BuildFor(offer, oldPriceList.Id)
                .WithPriceRow(id: oldPriceList.Rows.First().Id, maxIncludedCount: 42)
                .WithNewPriceRow(maxIncludedCount: 28);
            ShouldThrowWhenModify(oldPriceList, newPriceList2, offer, usageWithCount, t => t.PriceListChangedDespiteCount());
        }

        [Fact]
        public void ModifyPriceList_Validation_ShouldNotThrowWhen_TwoRowsAdded_OnTop_EvenWhenHavingACount()
        {
            var offer = new CommercialOffer().Build()
                .WithPriceList()
                .AndPriceRow(maxIncludedCount: 42);
            var oldPriceList = offer.PriceLists.First();
            var usageWithCount = new CommercialOfferUsage().BuildFor(offer)
                .WithCountedContractsNumber(1);

            var newPriceList = new PriceList().BuildFor(offer, oldPriceList.Id)
                .WithPriceRow(id: oldPriceList.Rows.First().Id, maxIncludedCount: 42)
                .WithNewPriceRow(maxIncludedCount: 9001)
                .WithNewPriceRow(maxIncludedCount: 9003);
            ShouldNotThrowWhenModify(oldPriceList, newPriceList, offer, usageWithCount);
        }

        [Fact]
        public void AddPriceList_Validation_ShouldThrowOnlyIf_StartDate_InThePast()
        {
            var offer = new CommercialOffer().Build();
            var priceList = new PriceList().CreateFor(offer)
                .StartingOn(new DateTime(2010, 01, 01));

            _time.Setup(time => time.Today())
                .Returns(new DateTime(2021, 01, 01));
            ShouldThrowWhenAdd(priceList, offer, t => t.PriceListStartDefinedBeforeThisMonth());

            Reset();

            _time.Setup(time => time.Today())
                .Returns(new DateTime(2009, 01, 01));
            ShouldNotThrowWhenAdd(priceList, offer);

            Reset();

            _time.Setup(time => time.Today())
                .Returns(new DateTime(2010, 01, 05));
            ShouldNotThrowWhenAdd(priceList, offer);
        }

        [Fact]
        public void ModifyPriceList_Validation_ShouldThrowOnlyIf_StartDateChanged_InThePast()
        {
            var offer = new CommercialOffer().Build()
                .WithPriceList()
                .WithPriceList(startingOn: new DateTime(2020, 01, 01));
            var oldPriceList = offer.PriceLists.Last();
            var newPriceList = new PriceList().BuildFor(offer, oldPriceList.Id)
                .StartingOn(new DateTime(2010, 01, 01));
            var usage = new CommercialOfferUsage().BuildFor(offer);

            _time.Setup(time => time.Today())
                .Returns(new DateTime(2021, 01, 01));
            ShouldThrowWhenModify(oldPriceList, newPriceList, offer, usage, t => t.PriceListStartDefinedBeforeThisMonth());

            Reset();

            _time.Setup(time => time.Today())
                .Returns(new DateTime(2009, 01, 01));
            ShouldNotThrowWhenModify(oldPriceList, newPriceList, offer, usage);

            Reset();

            _time.Setup(time => time.Today())
                .Returns(new DateTime(2010, 01, 05));
            ShouldNotThrowWhenModify(oldPriceList, newPriceList, offer, usage);
        }

        [Fact]
        public void ModifyPriceList_Validation_ShouldThrowOnlyIf_StartDateChanged_OnOldestPriceList()
        {
            var offer = new CommercialOffer().Build()
                .WithPriceList(startingOn: new DateTime(2020, 01, 01))
                .WithPriceList(startingOn: new DateTime(2021, 01, 01));
            var oldestPriceList = offer.PriceLists.OrderBy(pl => pl.StartsOn).First();
            var secondOldestPriceList = offer.PriceLists.OrderBy(pl => pl.StartsOn).Last();
            var usage = new CommercialOfferUsage().BuildFor(offer);
            _time.Setup(time => time.Today())
                .Returns(new DateTime(2009, 01, 01));

            var newPriceList = new PriceList().BuildFor(offer, oldestPriceList.Id)
                .StartingOn(new DateTime(2010, 01, 01));
            ShouldThrowWhenModify(oldestPriceList, newPriceList, offer, usage, t => t.OldestPriceListStartDateChanged());

            Reset();

            var newPriceList2 = new PriceList().BuildFor(offer, secondOldestPriceList.Id)
                .StartingOn(new DateTime(2010, 01, 01));
                ShouldNotThrowWhenModify(secondOldestPriceList, newPriceList, offer, usage);
        }

        [Fact]
        public void ModifyPriceList_Validation_ShouldThrowIf_OfferIdChanged()
        {
            var offer = new CommercialOffer().Build()
                .WithPriceList()
                .WithPriceList(startingOn: new DateTime(2010, 01, 01));
            var oldPriceList = offer.PriceLists.Last();
            var newPriceList = new PriceList().BuildFor(offer, oldPriceList.Id)
                .StartingOn(new DateTime(2010, 01, 01));
            var usage = new CommercialOfferUsage().BuildFor(offer);

            newPriceList.OfferId = -1;
            ShouldThrowWhenModify(oldPriceList, newPriceList, offer, usage, t => t.PriceListDetached());
        }

        [Fact]
        public void ModifyPriceList_Validation_ShouldThrowIf_RowListIdChanged()
        {
            var offer = new CommercialOffer().Build()
                .WithPriceList()
                .WithPriceList(startingOn: new DateTime(2010, 01, 01))
                .AndPriceRow();
            var oldPriceList = offer.PriceLists.Last();
            var newPriceList = new PriceList().BuildFor(offer, oldPriceList.Id)
                .StartingOn(new DateTime(2010, 01, 01))
                .WithPriceRow(id: oldPriceList.Rows.Last().Id);
            var usage = new CommercialOfferUsage().BuildFor(offer);

            newPriceList.Rows.Last().ListId = -1;
            ShouldThrowWhenModify(oldPriceList, newPriceList, offer, usage, t => t.PriceRowDetached());
        }

        [Fact]
        public void DeletePriceList_Validation_ShouldThrowOnlyIf_StartDateHasPassed()
        {
            var offer = new CommercialOffer().Build()
                .WithPriceList()
                .WithPriceList(startingOn: new DateTime(2010, 01, 01));
            var priceList = offer.PriceLists.Last();

            _time.Setup(time => time.Today())
                .Returns(new DateTime(2020, 01, 01));
            ShouldThrowWhenDelete(priceList, offer, t => t.StartedPriceListDeleted());

            Reset();

            _time.Setup(time => time.Today())
                .Returns(new DateTime(2009, 01, 01));
            ShouldNotThrowWhenDelete(priceList, offer);
        }

        [Fact]
        public void AddPriceList_ShouldThrowOnlyIf_ContainsNegativeAmount()
        {
            var offer = new CommercialOffer().Build();
            offer.PriceLists = new List<PriceList>();

            var list = new PriceList().StartingOn(new DateTime(2040, 01, 01)).WithPriceRow(0, 10, 0, 0);

            ShouldNotThrowWhenAdd(list, offer);
            list.Rows.First().FixedPrice = -0.01m;
            ShouldThrowWhenAdd(list, offer, t => t.PriceListHasNegativeAmounts());
        }

        private void Reset()
        {
            _translations.Reset();
        }

        private void ShouldThrowWhenCreate(CommercialOffer offer, Expression<Func<IContractsTranslations, string>> reasonFn)
        {
            Action throwIfFn = () => Validation.ThrowIfCannotCreateOffer(offer);

            throwIfFn.Should().ThrowExactly<OfferValidationException>();
            _translations.Verify(reasonFn, Times.Once);
        }

        private void ShouldNotThrowWhenCreate(CommercialOffer offer)
        {
            Action throwIfFn = () => Validation.ThrowIfCannotCreateOffer(offer);

            throwIfFn.Should().NotThrow<OfferValidationException>();
        }

        private void ShouldThrowWhenModify(CommercialOffer oldOffer, CommercialOffer newOffer, CommercialOfferUsage usage, Expression<Func<IContractsTranslations, string>> reasonFn)
        {
            Action throwIfFn = () => Validation.ThrowIfCannotModifyOffer(oldOffer, newOffer, usage);

            throwIfFn.Should().ThrowExactly<OfferValidationException>();
            _translations.Verify(reasonFn, Times.Once);
        }

        private void ShouldNotThrowWhenModify(CommercialOffer oldOffer, CommercialOffer newOffer, CommercialOfferUsage usage)
        {
            Action throwIfFn = () => Validation.ThrowIfCannotModifyOffer(oldOffer, newOffer, usage);

            throwIfFn.Should().NotThrow<OfferValidationException>();
        }

        private void ShouldThrowWhenAdd(PriceList pl, CommercialOffer offer, Expression<Func<IContractsTranslations, string>> reasonFn)
        {
            Action throwIfFn = () => Validation.ThrowIfCannotAddPriceList(offer, pl);

            throwIfFn.Should().ThrowExactly<OfferValidationException>();
            _translations.Verify(reasonFn, Times.Once);
        }

        private void ShouldNotThrowWhenAdd(PriceList pl, CommercialOffer offer)
        {
            Action throwIfFn = () => Validation.ThrowIfCannotAddPriceList(offer, pl);

            throwIfFn.Should().NotThrow<OfferValidationException>();
        }

        private void ShouldThrowWhenModify(PriceList pl, PriceList newPriceList, CommercialOffer offer, CommercialOfferUsage usage, Expression<Func<IContractsTranslations, string>> reasonFn)
        {
            Action throwIfFn = () => Validation.ThrowIfCannotModifyPriceList(offer, pl, newPriceList, usage);

            throwIfFn.Should().ThrowExactly<OfferValidationException>();
            _translations.Verify(reasonFn, Times.Once);
        }

        private void ShouldNotThrowWhenModify(PriceList pl, PriceList newPriceList, CommercialOffer offer, CommercialOfferUsage usage)
        {
            Action throwIfFn = () => Validation.ThrowIfCannotModifyPriceList(offer, pl, newPriceList, usage);

            throwIfFn.Should().NotThrow<OfferValidationException>();
        }

        private void ShouldThrowWhenDelete(PriceList pl, CommercialOffer offer, Expression<Func<IContractsTranslations, string>> reasonFn)
        {
            Action throwIfFn = () => Validation.ThrowIfCannotDeletePriceList(offer, pl);

            throwIfFn.Should().ThrowExactly<OfferValidationException>();
            _translations.Verify(reasonFn, Times.Once);
        }

        private void ShouldNotThrowWhenDelete(PriceList pl, CommercialOffer offer)
        {
            Action throwIfFn = () => Validation.ThrowIfCannotDeletePriceList(offer, pl);

            throwIfFn.Should().NotThrow<OfferValidationException>();
        }
    }
}
