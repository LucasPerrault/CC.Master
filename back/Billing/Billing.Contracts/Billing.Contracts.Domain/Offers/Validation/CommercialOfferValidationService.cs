using Billing.Contracts.Domain.Offers.Comparisons;
using Billing.Contracts.Domain.Offers.Validation.Exceptions;
using Resources.Translations;
using System;
using System.Collections.Generic;
using System.Linq;
using Tools;

namespace Billing.Contracts.Domain.Offers.Validation
{
    public class CommercialOfferValidationService
    {
        private readonly ITimeProvider _time;
        private readonly ITranslations _translations;

        public CommercialOfferValidationService(ITimeProvider time, ITranslations translations)
        {
            _time = time;
            _translations = translations;
        }

        public void ThrowIfCannotCreateOffer(CommercialOffer newOffer)
        {
            if (newOffer.PriceLists.Any(l => IsStartDateNotOnFirstDayOfTheMonth(l)))
            {
                throw new OfferValidationException(GetCreateOfferMessage(_translations.PriceListStartsOnFirstOfMonth()));
            }
            if (HasSameStartDateOnSeveralPriceLists(newOffer.PriceLists))
            {
                throw new OfferValidationException(GetCreateOfferMessage(_translations.PriceListsStartsOnSameDay()));
            }
        }

        public void ThrowIfCannotModifyOffer(CommercialOffer oldOffer, CommercialOffer newOffer, CommercialOfferUsage oldUsage)
        {
            if (HasContractWithCount(oldUsage) && HasAnyOfferPropertyChangedBesidesNameAndTag(oldOffer, newOffer))
            {
                throw new OfferValidationException(GetModifyOfferMessage(oldOffer.Id, _translations.OfferChangedDespiteCount()));
            }
            if (HasAnyPriceListLoaded(newOffer))
            {
                throw new OfferValidationException(GetModifyOfferMessage(oldOffer.Id, _translations.PriceListChanged()));
            }
        }

        public void ThrowIfCannotAddPriceList(CommercialOffer offer, PriceList priceList)
        {
            if (IsStartDateNotOnFirstDayOfTheMonth(priceList))
            {
                throw new OfferValidationException(GetCreatePriceListMessage(offer.Id, _translations.PriceListStartsOnFirstOfMonth()));
            }
            if (IsStartDateBeforeThisMonth(priceList))
            {
                throw new OfferValidationException(GetCreatePriceListMessage(offer.Id, _translations.PriceListStartDefinedBeforeThisMonth()));
            }
            if (HasSameStartDateOnSeveralPriceLists(offer.PriceLists.Union(new List<PriceList> { priceList })))
            {
                throw new OfferValidationException(GetCreatePriceListMessage(offer.Id, _translations.PriceListsStartsOnSameDay()));
            }
            if (!IsOrdered(priceList))
            {
                throw new OfferValidationException(GetCreatePriceListMessage(offer.Id, _translations.PriceRowsNotOrdered()));
            }
        }

        public void ThrowIfCannotModifyPriceList(CommercialOffer offer, PriceList oldPriceList, PriceList newPriceList, CommercialOfferUsage oldUsage)
        {
            if (HasNoRow(newPriceList))
            {
                throw new OfferValidationException(GetModifyPriceListMessage(oldPriceList.Id, offer.Id, _translations.PriceListShouldHaveRows()));
            }
            if (IsStartDateNotOnFirstDayOfTheMonth(newPriceList))
            {
                throw new OfferValidationException(GetModifyPriceListMessage(oldPriceList.Id, offer.Id, _translations.PriceListStartsOnFirstOfMonth()));
            }
            if (oldPriceList.StartsOn != newPriceList.StartsOn && IsStartDateBeforeThisMonth(newPriceList))
            {
                throw new OfferValidationException(GetModifyPriceListMessage(oldPriceList.Id, offer.Id, _translations.PriceListStartDefinedBeforeThisMonth()));
            }
            if (HasStartDateChangedOnOldestPriceList(offer, oldPriceList, newPriceList))
            {
                throw new OfferValidationException(GetModifyPriceListMessage(oldPriceList.Id, offer.Id, _translations.OldestPriceListStartDateChanged()));
            }
            if (HasSameStartDateOnSeveralPriceLists(offer.PriceLists.Union(new List<PriceList> { newPriceList })))
            {
                throw new OfferValidationException(GetModifyPriceListMessage(oldPriceList.Id, offer.Id, _translations.PriceListsStartsOnSameDay()));
            }
            if (HasContractWithCount(oldUsage) && HasPriceListChanged(oldPriceList, newPriceList, allowNewHigherRows: true))
            {
                throw new OfferValidationException(GetModifyPriceListMessage(oldPriceList.Id, offer.Id, _translations.PriceListChangedDespiteCount()));
            }
            if (HasOfferIdChanged(oldPriceList, newPriceList))
            {
                throw new OfferValidationException(GetModifyPriceListMessage(oldPriceList.Id, offer.Id, _translations.PriceListDetached()));
            }
            if (HasAnyPriceListIdChanged(oldPriceList, newPriceList))
            {
                throw new OfferValidationException(GetModifyPriceListMessage(oldPriceList.Id, offer.Id, _translations.PriceRowDetached()));
            }
            if (!IsOrdered(newPriceList))
            {
                throw new OfferValidationException(GetModifyPriceListMessage(oldPriceList.Id, offer.Id, _translations.PriceRowsNotOrdered()));
            }
        }

        public void ThrowIfCannotDeletePriceList(CommercialOffer offer, PriceList priceList)
        {
            if (HasStarted(priceList))
            {
                throw new OfferValidationException(GetDeletePriceListMessage(priceList.Id, offer.Id, _translations.StartedPriceListDeleted()));
            }
        }

        private bool HasStarted(PriceList priceList)
        {
            return priceList.StartsOn < _time.Today();
        }

        private bool IsStartDateBeforeThisMonth(PriceList priceList)
        {
            var today = _time.Today();
            var startOfThisMonth = today.AddDays(1 - today.Day);
            return priceList.StartsOn < startOfThisMonth;
        }

        private static bool HasNoRow(PriceList pl)
        {
            return pl.Rows == null;
        }

        private static bool IsStartDateNotOnFirstDayOfTheMonth(PriceList priceList)
        {
            return priceList.StartsOn.Day != 1;
        }

        private static bool HasSameStartDateOnSeveralPriceLists(IEnumerable<PriceList> lists)
        {
            return lists
                .GroupBy(pl => new { pl.StartsOn.Year, pl.StartsOn.Month })
                .Any(g => g.Select(pl => pl.Id).Distinct().Count() > 1);
        }

        private static bool HasContractWithCount(CommercialOfferUsage usage)
        {
            return usage != null && usage.NumberOfCountedContracts > 0;
        }

        private static bool HasAnyOfferPropertyChangedBesidesNameAndTag(CommercialOffer oldOffer, CommercialOffer newOffer)
        {
            var oldOfferComparison = new CommercialOfferComparisonObject(oldOffer);
            var newOfferComparison = new CommercialOfferComparisonObject(newOffer);

            return oldOfferComparison != newOfferComparison;
        }

        private static bool HasAnyPriceListLoaded(CommercialOffer newOffer)
        {
            return newOffer.PriceLists != null;
        }

        private static bool HasPriceListChanged(PriceList oldList, PriceList newList, bool allowNewHigherRows)
        {
            return HasAnyPriceListPropertyChanged(oldList, newList)
                || HasAnyRowChanged(oldList, newList, allowNewHigherRows);
        }

        private static bool HasAnyPriceListPropertyChanged(PriceList oldList, PriceList newList)
        {
            var oldListComparison = new PriceListComparisonObject(oldList);
            var newListComparison = new PriceListComparisonObject(newList);

            return oldListComparison != newListComparison;
        }

        private static bool HasAnyRowChanged(PriceList oldPriceList, PriceList newPriceList, bool allowNewHigherRows)
        {
            if (oldPriceList.Rows.Count > newPriceList.Rows.Count)
            {
                return true;
            }

            var sortedOldRows = oldPriceList.Rows.OrderBy(r => r.MaxIncludedCount).ToList();
            var sortedNewRows = newPriceList.Rows.OrderBy(r => r.MaxIncludedCount).ToList();

            if (sortedNewRows.Take(sortedOldRows.Count)
                .Zip(sortedOldRows)
                .Any(rowTuple => !AreEqual(rowTuple.First, rowTuple.Second)))
            {
                return true;
            }

            if (!allowNewHigherRows)
            {
                return sortedOldRows.Count != sortedNewRows.Count;
            }

            var oldLastRow = sortedOldRows.Last();
            var newRowOnTop = newPriceList.Rows.Count > oldPriceList.Rows.Count
                ? newPriceList.Rows[oldPriceList.Rows.Count]
                : null;

            return newRowOnTop != null && oldLastRow.MaxIncludedCount > newRowOnTop.MaxIncludedCount;
        }

        private static bool AreEqual(PriceRow r1, PriceRow r2)
        {
            var r1Comparison = new PriceRowComparisonObject(r1);
            var r2Comparison = new PriceRowComparisonObject(r2);

            return r1Comparison == r2Comparison;
        }

        private static bool HasStartDateChangedOnOldestPriceList(CommercialOffer offer, PriceList targetPriceList, PriceList newPriceList)
        {
            if (offer.PriceLists.Count == 0)
            {
                return false;
            }
            return offer.PriceLists.OrderBy(pl => pl.StartsOn).First().Id == targetPriceList.Id
                && targetPriceList.StartsOn != newPriceList.StartsOn;
        }

        private static bool HasOfferIdChanged(PriceList oldPriceList, PriceList newPriceList)
        {
            return oldPriceList.OfferId != newPriceList.OfferId;
        }

        private static bool HasAnyPriceListIdChanged(PriceList oldPriceList, PriceList newPriceList)
        {
            return newPriceList.Rows.Any(r => r.ListId != oldPriceList.Id);
        }

        private bool IsOrdered(PriceList priceList)
        {
            var previousMax = 0;
            foreach (var priceRow in priceList.Rows)
            {
                if (previousMax >= priceRow.MaxIncludedCount)
                {
                    return false;
                }

                previousMax = priceRow.MaxIncludedCount;
            }

            return true;
        }

        private string GetCreateOfferMessage(string reason)
            => _translations.CreateOfferExceptionMessage(reason);
        private string GetModifyOfferMessage(int offerId, string reason)
            => _translations.ModifyOfferExceptionMessage(offerId, reason);
        private string GetDeleteOfferMessage(int offerId, string reason)
            => _translations.DeleteOfferExceptionMessage(offerId, reason);
        private string GetCreatePriceListMessage(int offerId, string reason)
            => _translations.CreatePriceListExceptionMessage(offerId, reason);
        private string GetModifyPriceListMessage(int priceListId, int offerId, string reason)
            => _translations.ModifyPriceListExceptionMessage(priceListId, offerId, reason);
        private string GetDeletePriceListMessage(int priceListId, int offerId, string reason)
            => _translations.DeletePriceListExceptionMessage(priceListId, offerId, reason);
    }
}
