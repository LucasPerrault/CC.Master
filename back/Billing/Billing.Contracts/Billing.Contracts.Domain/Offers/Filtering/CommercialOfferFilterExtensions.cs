using System;
using System.Collections.Generic;
using System.Linq;

namespace Billing.Contracts.Domain.Offers.Filtering
{
    public static class CommercialOfferFilterExtensions
    {
        public static bool IsSimilarUntil(this CommercialOffer target, CommercialOffer reference, DateTime until)
        {
            if (target.Id == reference.Id)
            {
                return false;
            }

            var referencePriceListsUntil = reference.PriceLists
                .Where(pl => pl.StartsOn < until)
                .ToList();
            var targetPriceListsUntil = target.PriceLists
                .Where(pl => pl.StartsOn < until)
                .ToList();
            if (targetPriceListsUntil.Count != referencePriceListsUntil.Count)
            {
                return false;
            }

            var referencePriceListsByStartsOn = referencePriceListsUntil.ToDictionary(pl => pl.StartsOn, pl => pl);

            return targetPriceListsUntil.All(pl =>
                referencePriceListsByStartsOn.TryGetValue(pl.StartsOn, out var referencePriceList)
                && pl.IsIdenticalTo(referencePriceList)
            );
        }

        private static bool IsIdenticalTo(this PriceList target, PriceList reference)
        {
            if (target.Rows.Count < reference.Rows.Count)
            {
                return false;
            }

            return target.Rows.AreReferenceRowsPreserved(reference.Rows);
        }

        private static bool AreReferenceRowsPreserved(this IReadOnlyCollection<PriceRow> targetRows, IReadOnlyCollection<PriceRow> referenceRows)
        {
            var referenceRowsByMaxIncludedCount = referenceRows.ToDictionary(r => r.MaxIncludedCount, r => r);
            return targetRows
                .OrderBy(r => r.MaxIncludedCount)
                .Take(referenceRows.Count)
                .All(r =>
                    referenceRowsByMaxIncludedCount.TryGetValue(r.MaxIncludedCount, out var referenceRow)
                    && r.IsIdenticalTo(referenceRow)
                );
        }

        private static bool IsIdenticalTo(this PriceRow target, PriceRow reference)
        {
            return target.FixedPrice == reference.FixedPrice
                && target.UnitPrice == reference.UnitPrice;
        }
    }
}
