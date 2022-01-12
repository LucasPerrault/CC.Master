using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Counts.Interfaces;
using Billing.Contracts.Domain.Offers;
using Billing.Contracts.Domain.Offers.Interfaces;
using System;
using Tools;

namespace Billing.Contracts.Domain.Counts.CreationStrategy
{
    public class CountCreationStrategyService
    {
        private readonly PriceListService _priceListService;
        private readonly MinimalBillingService _minimalBillingService;
        private readonly ITimeProvider _time;
        private readonly ICountsStore _countsStore;
        private readonly IContractPricingsStore _contractPricingsStore;

        public CountCreationStrategyService
        (
            PriceListService priceListService,
            MinimalBillingService minimalBillingService,
            ITimeProvider time,
            ICountsStore countsStore,
            IContractPricingsStore contractPricingsStore
        )
        {
            _priceListService = priceListService;
            _minimalBillingService = minimalBillingService;
            _time = time;
            _countsStore = countsStore;
            _contractPricingsStore = contractPricingsStore;
        }

        public CountCreationStrategy GetCountCreationStrategy(ContractWithCountNumber contractWithCountNumber)
        {
            return ShouldCreateRealCount(contractWithCountNumber.Period)
                ? GetRealCountCreationStrategy(contractWithCountNumber)
                : GetForecastCountCreationStrategy(contractWithCountNumber);
        }

        private bool ShouldCreateRealCount(AccountingPeriod period)
        {
            var currentPeriod = _time.Today();

            if (period.Year < currentPeriod.Year)
            {
                return true;
            }
            if (period.Year == currentPeriod.Year)
            {
                return period.Month < currentPeriod.Month;
            }
            return false;
        }

        private CountCreationStrategy GetRealCountCreationStrategy(ContractWithCountNumber contractWithCountNumber)
        {

            return contractWithCountNumber.Contract.CommercialOffer.PricingMethod switch
            {
                PricingMethod.Linear => new StandardCountCreationStrategy(_priceListService, _minimalBillingService),
                PricingMethod.AnnualCommitment => throw new ObsoletePricingMethodException(),
                PricingMethod.Constant => new ConstantCountCreationStrategy(_contractPricingsStore),
                _ => throw new ApplicationException($"No count strategy corresponds to pricing method {contractWithCountNumber.Contract.CommercialOffer.PricingMethod}"),
            };
        }

        private CountCreationStrategy GetForecastCountCreationStrategy(ContractWithCountNumber contractWithCountNumber)
        {
            return contractWithCountNumber.Contract.CommercialOffer.ForecastMethod switch
            {
                ForecastMethod.AnnualCommitment => throw new ObsoletePricingMethodException(),
                ForecastMethod.LastYear => throw new ObsoletePricingMethodException(),
                ForecastMethod.LastRealMonth => new LastRealMonthCountCreationStrategy(_countsStore),
                _ => throw new ApplicationException($"No count strategy corresponds to forecast method {contractWithCountNumber.Contract.CommercialOffer.ForecastMethod}"),
            };
        }

        private class ObsoletePricingMethodException : ApplicationException
        {
            public ObsoletePricingMethodException() : base("Obsolete pricing method")
            { }
        }
    }
}
