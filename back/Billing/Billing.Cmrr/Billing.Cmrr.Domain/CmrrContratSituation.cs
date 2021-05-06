namespace Billing.Cmrr.Domain
{
    public class CmrrContratSituation
    {
        public int ContractId { get; }
        public CmrrContract Contract { get; }

        public CmrrCount StartPeriodCount { get; }
        public CmrrCount EndPeriodCount { get; }

        public CmrrLifeCycle LifeCycle { get; }

        private const int MaxMonthDurationForUpsell = 6;

        public CmrrContratSituation(CmrrContract contract, CmrrCount startPeriodCount, CmrrCount endPeriodCount)
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
                if (contract.EndReason == ContractEndReason.Modification)
                    return CmrrLifeCycle.Retraction;
                return CmrrLifeCycle.Termination;
            }

            if (startPeriodCount is null)
            {
                var isModification = contract.CreationCause == ContractCreationCause.Modification;
                if (!isModification && contract.EnvironmentCreatedAt.AddMonths(MaxMonthDurationForUpsell) < contract.StartDate)
                    return CmrrLifeCycle.Upsell;
                return CmrrLifeCycle.Creation;
            }

            if (startPeriodCount.EuroTotal >= endPeriodCount.EuroTotal)
                return CmrrLifeCycle.Retraction;

            return CmrrLifeCycle.Expansion;
        }
    }
}
