using Billing.Contracts.Domain.Offers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Billing.Contracts.Domain.Tests
{
    public static class OfferTestDataBuildingExtensions
    {
        private static int _offerNewId = 1;
        private static int _priceListNewId = 1;
        private static int _priceRowNewId = 1;

        public static CommercialOffer Build(this CommercialOffer offer, int? id = null)
        {
            offer.Id = id ?? _offerNewId++;
            return offer;
        }

        public static CommercialOffer With(this CommercialOffer offer,
            string name = default,
            string tag = default,
            BillingMode billingMode = default,
            BillingUnit unit = default,
            PricingMethod pricingMethod = default,
            ForecastMethod forecastMethod = default,
            bool isArchived = default,
            int productId = default,
            int currencyId = default
        )
        {
            offer.Name = name;
            offer.Tag = tag;
            offer.BillingMode = billingMode;
            offer.Unit = unit;
            offer.PricingMethod = pricingMethod;
            offer.ForecastMethod = forecastMethod;
            offer.IsArchived = isArchived;
            offer.ProductId = productId;
            offer.CurrencyId = currencyId;

            return offer;
        }

        public static CommercialOffer WithPriceList(this CommercialOffer offer, DateTime startingOn = default)
        {
            var pl = new PriceList().BuildFor(offer).StartingOn(startingOn);

            offer.PriceLists ??= new List<PriceList>();
            offer.PriceLists.Add(pl);
            return offer;
        }

        public static CommercialOffer AndPriceRow(this CommercialOffer offer, int maxExcludedCount = default, decimal unitPrice = default, decimal fixedPrice = default)
        {
            var pl = offer.PriceLists.Last();

            var pr = new PriceRow
            {
                Id = _priceRowNewId++,
                ListId = pl.Id,
                MaxIncludedCount = maxExcludedCount,
                UnitPrice = unitPrice,
                FixedPrice = fixedPrice,
            };

            pl.Rows.Add(pr);
            return offer;
        }

        public static PriceList BuildFor(this PriceList priceList, CommercialOffer offer, int? id = null)
        {
            priceList.Id = id ?? _priceListNewId++;
            priceList.Rows = new List<PriceRow>();
            priceList.OfferId = offer.Id;
            return priceList;
        }

        public static PriceList CreateFor(this PriceList priceList, CommercialOffer offer)
        {
            var newPl = priceList.BuildFor(offer);
            newPl.Id = 0;

            return newPl;
        }

        public static PriceList StartingOn(this PriceList pl, DateTime startingOn)
        {
            pl.StartsOn = startingOn;
            return pl;
        }

        public static PriceList WithoutPriceRow(this PriceList pl)
        {
            pl.Rows = null;
            return pl;
        }

        public static PriceList WithPriceRow(this PriceList pl, int? id = null, int maxExcludedCount = default, decimal unitPrice = default, decimal fixedPrice = default)
        {
            var pr = new PriceRow
            {
                Id = id ?? _priceRowNewId++,
                ListId = pl.Id,
                MaxIncludedCount = maxExcludedCount,
                UnitPrice = unitPrice,
                FixedPrice = fixedPrice
            };

            pl.Rows ??= new List<PriceRow>();
            pl.Rows.Add(pr);
            return pl;
        }

        public static PriceList WithNewPriceRow(this PriceList pl, int maxExcludedCount = default, decimal unitPrice = default, decimal fixedPrice = default)
        {
            var modifiedPl = pl.WithPriceRow(maxExcludedCount: maxExcludedCount, unitPrice: unitPrice, fixedPrice: fixedPrice);
            modifiedPl.Rows.Last().Id = 0;

            return modifiedPl;
        }

        public static CommercialOfferUsage BuildFor(this CommercialOfferUsage usage, CommercialOffer offer)
        {
            usage.OfferId = offer.Id;
            return usage;
        }

        public static CommercialOfferUsage WithActiveContractsNumber(this CommercialOfferUsage usage, int nbActiveContracts)
        {
            usage.NumberOfActiveContracts = nbActiveContracts;
            return usage;
        }

        public static CommercialOfferUsage WithCountedContractsNumber(this CommercialOfferUsage usage, int nbCountedContracts)
        {
            usage.NumberOfCountedContracts = nbCountedContracts;
            return usage;
        }
    }
}
