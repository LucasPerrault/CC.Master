using Billing.Contracts.Domain.Offers.Comparisons;
using Billing.Contracts.Domain.Offers.Interfaces;
using Billing.Contracts.Domain.Offers.Validation.Exceptions;
using Resources.Translations;
using System.Collections.Generic;
using System.Linq;
using Tools;

namespace Billing.Contracts.Domain.Offers.Validation
{
    public class CommercialOfferValidationService : ICommercialOfferValidationService
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
            if (HasContractWithCount(oldUsage) && HasAnyOfferPropertyChangedBesidesName(oldOffer, newOffer))
            {
                throw new OfferValidationException(GetModifyOfferMessage(oldOffer.Id, _translations.OfferChangedDespiteCount()));
            }
            if (HasAnyOfferPriceListChanged(oldOffer, newOffer))
            {
                throw new OfferValidationException(GetModifyOfferMessage(oldOffer.Id, _translations.PriceListChanged()));
            }
        }

        public void ThrowIfCannotDeleteOffer(CommercialOffer offer, CommercialOfferUsage usage)
        {
            if (HasActiveContract(usage))
            {
                throw new OfferValidationException(GetDeleteOfferMessage(offer.Id, _translations.OfferWithActiveContractDeleted()));
            }
        }

        public void ThrowIfCannotAddPriceList(CommercialOffer offer, PriceList priceList)
        {
            if (IsStartDateNotOnFirstDayOfTheMonth(priceList))
            {
                throw new OfferValidationException(GetCreatePriceListMessage(offer.Id, _translations.PriceListStartsOnFirstOfMonth()));
            }
            if (IsStartDateInThePast(priceList))
            {
                throw new OfferValidationException(GetCreatePriceListMessage(offer.Id, _translations.PriceListStartDefinedInThePast()));
            }
            if (HasSameStartDateOnSeveralPriceLists(offer.PriceLists.Union(new List<PriceList> { priceList })))
            {
                throw new OfferValidationException(GetCreatePriceListMessage(offer.Id, _translations.PriceListsStartsOnSameDay()));
            }
        }

        public void ThrowIfCannotModifyPriceList(CommercialOffer offer, PriceList oldPriceList, PriceList newPriceList, CommercialOfferUsage oldUsage)
        {
            if (IsStartDateNotOnFirstDayOfTheMonth(newPriceList))
            {
                throw new OfferValidationException(GetModifyPriceListMessage(oldPriceList.Id, offer.Id, _translations.PriceListStartsOnFirstOfMonth()));
            }
            if (oldPriceList.StartsOn != newPriceList.StartsOn && IsStartDateInThePast(newPriceList))
            {
                throw new OfferValidationException(GetModifyPriceListMessage(oldPriceList.Id, offer.Id, _translations.PriceListStartDefinedInThePast()));
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
        }

        public void ThrowIfCannotDeletePriceList(CommercialOffer offer, PriceList priceList)
        {
            if (HasStarted(priceList))
            {
                throw new OfferValidationException(GetDeletePriceListMessage(priceList.Id, offer.Id, _translations.StartedPriceListDeleted()));
            }
        }

        private bool IsStartDateNotOnFirstDayOfTheMonth(PriceList priceList)
        {
            return priceList.StartsOn.Day != 1;
        }

        private bool HasSameStartDateOnSeveralPriceLists(IEnumerable<PriceList> lists)
        {
            return lists
                .GroupBy(pl => new { pl.StartsOn.Year, pl.StartsOn.Month })
                .Any(g => g.Select(pl => pl.Id).Distinct().Count() > 1);
        }

        private bool HasContractWithCount(CommercialOfferUsage usage)
        {
            return usage.NumberOfCountedContracts > 0;
        }

        private bool HasAnyOfferPropertyChangedBesidesName(CommercialOffer oldOffer, CommercialOffer newOffer)
        {
            var oldOfferComparison = new CommercialOfferComparisonObject(oldOffer);
            var newOfferComparison = new CommercialOfferComparisonObject(newOffer);

            return oldOfferComparison != newOfferComparison;
        }

        private bool HasAnyOfferPriceListChanged(CommercialOffer oldOffer, CommercialOffer newOffer)
        {
            var oldPriceListsById = oldOffer.PriceLists.ToDictionary(pl => pl.Id, pl => pl);
            var newPriceListsById = newOffer.PriceLists.ToDictionary(pl => pl.Id, pl => pl);

            return oldPriceListsById.Keys.Any(id =>
                !newPriceListsById.ContainsKey(id)
                || HasPriceListChanged(oldPriceListsById[id], newPriceListsById[id], allowNewHigherRows: false)
            );
        }

        private bool HasPriceListChanged(PriceList oldList, PriceList newList, bool allowNewHigherRows)
        {
            return HasAnyPriceListPropertyChanged(oldList, newList)
                || HasAnyRowChanged(oldList, newList, allowNewHigherRows);
        }

        private bool HasAnyPriceListPropertyChanged(PriceList oldList, PriceList newList)
        {
            var oldListComparison = new PriceListComparisonObject(oldList);
            var newListComparison = new PriceListComparisonObject(newList);

            return oldListComparison != newListComparison;
        }

        private bool HasAnyRowChanged(PriceList oldPriceList, PriceList newPriceList, bool allowNewHigherRows)
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

        private bool AreEqual(PriceRow r1, PriceRow r2)
        {
            var r1Comparison = new PriceRowComparisonObject(r1);
            var r2Comparison = new PriceRowComparisonObject(r2);

            return r1Comparison == r2Comparison;
        }

        private bool IsStartDateInThePast(PriceList priceList)
        {
            var today = _time.Today();
            return priceList.StartsOn < today;
        }

        private bool HasStartDateChangedOnOldestPriceList(CommercialOffer offer, PriceList targetPriceList, PriceList newPriceList)
        {
            if (offer.PriceLists.Count == 0)
            {
                return false;
            }
            return offer.PriceLists.OrderBy(pl => pl.StartsOn).First().Id == targetPriceList.Id
                && targetPriceList.StartsOn != newPriceList.StartsOn;
        }

        private bool HasOfferIdChanged(PriceList oldPriceList, PriceList newPriceList)
        {
            return oldPriceList.OfferId != newPriceList.OfferId;
        }

        private bool HasAnyPriceListIdChanged(PriceList oldPriceList, PriceList newPriceList)
        {
            var oldListIdsByRowId = oldPriceList.Rows.ToDictionary(pr => pr.Id, pr => pr.ListId);
            var newListIdsByRowId = newPriceList.Rows.ToDictionary(pr => pr.Id, pr => pr.ListId);
            return oldListIdsByRowId.Keys.Any(rowId =>
                newListIdsByRowId.ContainsKey(rowId)
                && oldListIdsByRowId[rowId] != newListIdsByRowId[rowId]
            );
        }

        private bool HasStarted(PriceList priceList)
        {
            return priceList.StartsOn < _time.Today();
        }

        private bool HasActiveContract(CommercialOfferUsage usage)
        {
            return usage.NumberOfActiveContracts > 0;
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
