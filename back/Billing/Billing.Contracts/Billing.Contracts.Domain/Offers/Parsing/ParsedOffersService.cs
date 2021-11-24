using Billing.Contracts.Domain.Offers.Parsing.Exceptions;
using Billing.Products.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Billing.Contracts.Domain.Offers.Parsing
{
    public class ParsedOffersService
    {
        public List<ParsedOffer> ConvertToParsedOffers(List<OfferRow> offerRows, List<Product> products)
        {
            var parsedOffers = new List<ParsedOffer>();

            ParsedOffer currentOffer = null;
            ParsedPriceList currentPriceList = null;
            var lastMaxIncludedCount = -1;
            var currentLine = 1;
            foreach (var offerRow in offerRows)
            {
                var isNewOffer = !string.IsNullOrEmpty(offerRow.Name);
                if (isNewOffer)
                {
                    currentOffer = new ParsedOffer(offerRow, products.SingleOrDefault(p => p.Id == offerRow.ProductId));
                    parsedOffers.Add(currentOffer);
                    if (!offerRow.StartsOn.HasValue)
                        throw new OfferRowStartsOnException(currentLine);
                }

                var isNewPriceList = offerRow.StartsOn.HasValue;

                var priceRow = new ParsedPriceRow
                {
                    MaxIncludedCount = offerRow.MaxIncludedCount,
                    FixedPrice = offerRow.FixedPrice,
                    UnitPrice = offerRow.UnitPrice
                };

                if (isNewPriceList)
                {
                    lastMaxIncludedCount = -1;
                    currentPriceList = new ParsedPriceList
                    {
                        StartsOn = offerRow.StartsOn,
                        Rows = new List<ParsedPriceRow> { priceRow }
                    };


                    currentOffer.PriceLists.Add(currentPriceList);
                }
                else
                    currentPriceList.Rows.Add(priceRow);

                if (offerRow.MinIncludedCount != lastMaxIncludedCount + 1)
                    throw new PriceRowsCoherencyException(currentLine, lastMaxIncludedCount, offerRow.MinIncludedCount);

                lastMaxIncludedCount = offerRow.MaxIncludedCount;

                currentLine++;
            }

            return parsedOffers;
        }
    }
}
