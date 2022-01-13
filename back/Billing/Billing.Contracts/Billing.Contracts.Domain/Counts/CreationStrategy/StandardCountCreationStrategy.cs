using Billing.Contracts.Domain.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Counts.CreationStrategy
{
    public class StandardCountCreationStrategy : CountCreationStrategy
    {
        private readonly PriceListService _priceListService;
        private readonly MinimalBillingService _minimalBillingService;

        public StandardCountCreationStrategy(PriceListService priceListService, MinimalBillingService minimalBillingService)
        {
            _priceListService = priceListService;
            _minimalBillingService = minimalBillingService;
        }

        protected override Task<CountCreationResult> MakeCountAsync(AccountingPeriod countPeriod, ContractWithCountNumber contractWithCountNumber, List<ContractWithCountNumber> otherFromContractGroup)
        {

            var otherCountNumberFromContractGroup = otherFromContractGroup.Select(c => c.CountNumber).ToList();

            var priceList = _priceListService.GetPriceListForCount(contractWithCountNumber.Contract, contractWithCountNumber.Period);
            var countContext = new CountContext
            {
                Contract = contractWithCountNumber.Contract,
                CountPeriod = countPeriod,
                PriceList = priceList,
                OtherCountNumberFromContractGroup = otherCountNumberFromContractGroup,
            };
            var count = MakeCountAsync(countContext, contractWithCountNumber.CountNumber);

            if (!_minimalBillingService.ShouldApplyMinimalBillingForPeriod(contractWithCountNumber.Contract, countPeriod))
            {
                return Task.FromResult(CountCreationResult.Success(count));
            }

            var minimalBillingCount = MakeMinimalBillingCountAsync(countContext);
            minimalBillingCount.IsMinimalBilling = true;

            var highPriorityCount = new[] {count, minimalBillingCount}.GetHighestPriority();
            return Task.FromResult(CountCreationResult.Success(highPriorityCount));
        }

        private Count MakeCountAsync(CountContext contractWithCountNumber, int countNumber)
        {
            var priceRow = _priceListService.GetPriceRow(contractWithCountNumber.PriceList, contractWithCountNumber.OtherCountNumberFromContractGroup.Sum() + countNumber);
            return new Count
            {
                ContractId = contractWithCountNumber.Contract.Id,
                CountPeriod = contractWithCountNumber.CountPeriod,
                Number = countNumber,
                UnitPrice = priceRow.UnitPrice,
                FixedPrice = priceRow.FixedPrice,

            };
        }

        private Count MakeMinimalBillingCountAsync(CountContext contractWithCountNumber)
        {
            var estimatedNumber = contractWithCountNumber.Contract.CountEstimation;
            var priceRow = _priceListService.GetPriceRow(contractWithCountNumber.PriceList, contractWithCountNumber.OtherCountNumberFromContractGroup.Sum() + estimatedNumber);
            var priceWithoutMinimalBilling = priceRow.FixedPrice + priceRow.UnitPrice * estimatedNumber;
            return new Count
            {
                ContractId = contractWithCountNumber.Contract.Id,
                CountPeriod = contractWithCountNumber.CountPeriod,
                Number = estimatedNumber,
                UnitPrice = 0,
                FixedPrice = priceWithoutMinimalBilling * (decimal)contractWithCountNumber.Contract.MinimalBillingPercentage / 100,
            };
        }
    }
}
