using Billing.Cmrr.Domain.Situation;

namespace Billing.Cmrr.Domain
{
    public enum CmrrLifeCycle
    {
        Upsell = 1,
        Creation = 2,
        Expansion = 3,
        Contraction = 4,
        Termination = 5
    }

    public class CmrrContractSituation
    {
        public int ContractId { get; }
        public CmrrContract Contract { get; }
        public CmrrCount StartPeriodCount { get; }
        public CmrrCount EndPeriodCount { get; }
        public CmrrLifeCycle LifeCycle { get; }

        private const int MaxMonthDurationForUpsell = 6;

        public CmrrContractSituation(CmrrContract contract, CmrrCount startPeriodCount, CmrrCount endPeriodCount)
        {
            Contract = contract;
            ContractId = contract.Id;
            StartPeriodCount = startPeriodCount;
            EndPeriodCount = endPeriodCount;

            LifeCycle = GetCmrrLifeCycle(contract, startPeriodCount, endPeriodCount);
        }

        private static CmrrLifeCycle GetCmrrLifeCycle(CmrrContract contract, CmrrCount startPeriodCount, CmrrCount endPeriodCount)
        {
            if (endPeriodCount is null)
            {
                if (contract.EndReason == ContractEndReason.Resiliation)
                    return CmrrLifeCycle.Termination;
                return CmrrLifeCycle.Contraction;
            }

            if (startPeriodCount is null)
            {
                var isModification = contract.CreationCause == ContractCreationCause.Modification;
                if (!isModification && contract.EnvironmentCreatedAt.AddMonths(MaxMonthDurationForUpsell) < contract.StartDate)
                    return CmrrLifeCycle.Upsell;
                return CmrrLifeCycle.Creation;
            }

            if (startPeriodCount.EuroTotal >= endPeriodCount.EuroTotal)
                return CmrrLifeCycle.Contraction;

            return CmrrLifeCycle.Expansion;
        }
    }

    public class ContractAxisSectionSituation
    {
        public const int RoundingDecimal = 2;
        public Breakdown Breakdown { get; }
        public CmrrContractSituation ContractSituation { get; }

        public decimal StartPeriodAmount { get; }
        public decimal EndPeriodAmount { get; }
        public decimal PartialDiff => EndPeriodAmount - StartPeriodAmount;

        public int StartPeriodUserCount { get; }
        public int EndPeriodUserCount { get; }
        public int UserCountDiff => EndPeriodUserCount - StartPeriodUserCount;

        public ContractAxisSectionSituation(Breakdown breakdown, CmrrContractSituation contractSituation)
        {
            Breakdown = breakdown;
            ContractSituation = contractSituation;

            StartPeriodAmount = decimal.Round(breakdown.Ratio * contractSituation.StartPeriodCount?.EuroTotal ?? 0, RoundingDecimal);
            EndPeriodAmount = decimal.Round(breakdown.Ratio * contractSituation.EndPeriodCount?.EuroTotal ?? 0, RoundingDecimal);

            StartPeriodUserCount = contractSituation.StartPeriodCount?.AccountingNumber ?? 0;
            EndPeriodUserCount = contractSituation.EndPeriodCount?.AccountingNumber ?? 0;
        }
    }
}
