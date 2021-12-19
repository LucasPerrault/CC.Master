using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Offers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Billing.Contracts.Domain.Counts
{
    public class PriceListService
    {
        public PriceList GetPriceListForCount(Contract contract, AccountingPeriod period)
        {
            return GetPriceList(contract.CommercialOffer.PriceLists, contract.TheoreticalStartOn.Month, period);
        }

        public PriceRow GetPriceRow(PriceList priceList, int number)
        {
            return priceList.Rows
                .OrderBy(l => l.MaxIncludedCount)
                .First(l => l.MaxIncludedCount >= number);
        }

        public PriceRow GetPriceRowOrDefault(PriceList priceList, int number)
        {
            return priceList.Rows
                .OrderBy(l => l.MaxIncludedCount)
                .FirstOrDefault(l => l.MaxIncludedCount >= number);
        }

        private PriceList GetPriceList(List<PriceList> priceLists, int contractBirthdayMonth, DateTime period)
        {
            var priceListDeterminationPeriod = GetDeterminationPeriod(period, contractBirthdayMonth);
            return priceLists
                .Where(l => l.StartsOn <= priceListDeterminationPeriod)
                .OrderByDescending(a => a.StartsOn)
                .First();
        }

        private AccountingPeriod GetDeterminationPeriod(DateTime period, int contractBirthdayMonth)
        {
            return contractBirthdayMonth <= period.Month
                ? new AccountingPeriod(period.Year, contractBirthdayMonth)
                : new AccountingPeriod(period.Year - 1, contractBirthdayMonth);
        }
    }
}
