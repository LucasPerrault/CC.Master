﻿using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Offers;
using Billing.Contracts.Domain.Offers.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Counts.CreationStrategy
{
    public class ConstantCountCreationStrategy : CountCreationStrategy
    {
        private readonly IContractPricingsStore _contractPricingsStore;

        public ConstantCountCreationStrategy(IContractPricingsStore contractPricingsStore)
        {
            _contractPricingsStore = contractPricingsStore;
        }
        public override async Task<Count> MakeCountAsync(AccountingPeriod countPeriod, ContractWithCountNumber contractWithCountNumber, List<ContractWithCountNumber> otherFromContractGroup)
        {
            var pricing = (await _contractPricingsStore.GetAsync())
                .Single(p => p.ContractId == contractWithCountNumber.Contract.Id && p.PricingMethod == PricingMethod.Constant);

            return new Count
            {
                ContractId = contractWithCountNumber.Contract.Id,
                CommercialOfferId = contractWithCountNumber.Contract.CommercialOfferId,
                FixedPrice = pricing.ConstantPrice,
                UnitPrice = 0,
                IsMinimalBilling = false,
                CountPeriod = countPeriod,
                Number = contractWithCountNumber.CountNumber,
            };
        }
    }
}
