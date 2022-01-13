using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Contracts;
using Lucca.Core.Shared.Domain.Exceptions;

namespace Billing.Contracts.Domain.Counts
{
    public class MinimalBillingService
    {
        public bool IsEligibleForMinimalBilling(Contract contract)
        {
            return !HasTheoreticalMonthRebate(contract)
                   && IsProductEligible(contract)
                   && IsDistributorEnforcingMinimalBilling(contract);
        }

        public bool ShouldApplyMinimalBillingForPeriod(Contract contract, AccountingPeriod countPeriod)
        {
            return IsInApplicablePeriod(contract, countPeriod)
                   && HasMinimalBilling(contract)
                   && IsEligibleForMinimalBilling(contract);
        }

        private bool IsInApplicablePeriod(Contract contract, AccountingPeriod countPeriod)
        {
            return contract.TheoreticalStartOn <= countPeriod
                   && countPeriod < contract.TheoreticalStartOn.AddYears(1);
        }

        private bool HasTheoreticalMonthRebate(Contract contract)
        {
            return contract.TheoreticalFreeMonths > 0;
        }

        private bool IsProductEligible(Contract contract)
        {
            return contract.CommercialOffer.Product?.IsEligibleToMinimalBilling ?? false;
        }

        private bool HasMinimalBilling(Contract contract)
        {
            return contract.MinimalBillingPercentage > 0;
        }

        private bool IsDistributorEnforcingMinimalBilling(Contract contract)
        {
            return contract.DistributorBillingPreference.IsEnforcingMinimalBilling;
        }

        public void ThrowIfPercentageAndEligibilityAreIncoherent(Contract contractOrDraft)
        {
            var isEligible = IsEligibleForMinimalBilling(contractOrDraft);

            var isNotEligibleWithMinBilling = !isEligible && contractOrDraft.MinimalBillingPercentage > 0;
            if (isNotEligibleWithMinBilling)
            {
                throw new BadRequestException("Ineligible contract must have no minimal billing.");
            }
        }
    }
}
