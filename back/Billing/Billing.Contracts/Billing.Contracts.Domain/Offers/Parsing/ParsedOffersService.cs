using Billing.Contracts.Domain.Offers.Parsing.Exceptions;
using Billing.Products.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Billing.Contracts.Domain.Offers.Parsing
{
    public class ParsedOffersService
    {
        public List<ParsedOffer> ConvertToParsedOffers(List<OfferRow> offerRows, List<Product> products)
        {
            return ConvertToParsedOffersEnumerable(offerRows, products).ToList();
        }

        private IEnumerable<ParsedOffer> ConvertToParsedOffersEnumerable(List<OfferRow> offerRows, List<Product> products)
        {
            var builder = new ParsedOfferIterator(offerRows);
            while (builder.TryGetNext(out var row, _ => true))
            {
                if (string.IsNullOrEmpty(row.Name))
                {
                    throw new OfferRowStartsOnException(builder.Index);
                }
                yield return new ParsedOffer(row, products.SingleOrDefault(p => p.Name == row.ProductName))
                {
                    PriceLists = GetPriceLists(row, builder).ToList(),
                };
            }
        }

        private IEnumerable<ParsedPriceList> GetPriceLists(OfferRow firstRow, ParsedOfferIterator iterator)
        {
            yield return new ParsedPriceList
            {
                StartsOn = firstRow.StartsOn.Value,
                Rows = GetPriceRows(firstRow, iterator).ToList(),
            };
            while (iterator.TryGetNext(out var row, r => string.IsNullOrEmpty(r.Name)))
            {
                if (!row.StartsOn.HasValue)
                {
                    throw new OfferRowStartsOnException(iterator.Index);
                }
                yield return new ParsedPriceList
                {
                    StartsOn = row.StartsOn,
                    Rows = GetPriceRows(row, iterator).ToList(),
                };
            }
        }

        private static IEnumerable<ParsedPriceRow> GetPriceRows(OfferRow firstRow, ParsedOfferIterator iterator)
        {
            if (firstRow.MinIncludedCount != 0)
            {
                throw new PriceRowsCoherencyException(iterator.Index, -1, firstRow.MinIncludedCount);
            }
            yield return new ParsedPriceRow
            {
                MaxIncludedCount = firstRow.MaxIncludedCount,
                FixedPrice = firstRow.FixedPrice,
                UnitPrice = firstRow.UnitPrice,
            };

            var lastMaxIncludedCount = firstRow.MaxIncludedCount;
            while (iterator.TryGetNext(out var row, r => !r.StartsOn.HasValue))
            {
                if (row.MinIncludedCount != lastMaxIncludedCount + 1)
                {
                    throw new PriceRowsCoherencyException(iterator.Index, lastMaxIncludedCount, row.MinIncludedCount);
                }
                yield return new ParsedPriceRow
                {
                    MaxIncludedCount = row.MaxIncludedCount,
                    FixedPrice = row.FixedPrice,
                    UnitPrice = row.UnitPrice,
                };
                lastMaxIncludedCount = row.MaxIncludedCount;
            }
        }

        public class ParsedOfferIterator
        {
            public int Index { get; private set; }
            private readonly List<OfferRow> _rows;

            public ParsedOfferIterator(List<OfferRow> rows) => _rows = rows;

            public bool TryGetNext(out OfferRow row, Func<OfferRow, bool> condition)
            {
                if (Index < _rows.Count)
                {
                    var candidateRow = _rows[Index];
                    if (condition(candidateRow))
                    {
                        row = candidateRow;
                        Index++;
                        return true;
                    }
                }

                row = null;
                return false;
            }
        }
    }
}
