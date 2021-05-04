namespace Billing.Cmrr.Domain
{
    public class CmrrContratSituation
    {
        public int ContractId { get; set; }
        public CmrrContract Contract { get; set; }

        public CmrrCount StartPeriodCount { get; set; }
        public CmrrCount EndPeriodCount { get; set; }

        public CmrrLifeCycle LifeCycle => GetCmrrLifeCycle();

        private readonly int _maxMonthDurationForUpsell = 6;
        
        private CmrrLifeCycle GetCmrrLifeCycle()
        {
            if(EndPeriodCount is null)
            {
                if (Contract.EndReason == ContractEndReason.Modification)
                    return CmrrLifeCycle.Retraction;
                return CmrrLifeCycle.Termination;
            }

            if(StartPeriodCount is null)
            {
                var isModification = Contract.CreationCause == ContractCreationCause.Modification;
                    if (isModification && Contract.EnvironmentCreatedAt.AddMonths(_maxMonthDurationForUpsell) < Contract.StartDate)
                        return CmrrLifeCycle.Upsell;
                return CmrrLifeCycle.Creation;
            }

            if(StartPeriodCount.EuroTotal >= EndPeriodCount.EuroTotal)
                return CmrrLifeCycle.Retraction;

            return CmrrLifeCycle.Expansion;
        }
    }
}
