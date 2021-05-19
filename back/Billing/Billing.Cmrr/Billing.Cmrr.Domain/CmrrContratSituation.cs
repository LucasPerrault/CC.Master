namespace Billing.Cmrr.Domain
{
    public class CmrrContratSituation
    {
        public int ContractId { get; set; }
        public CmrrContract Contract { get; set; }

        public CmrrCount StartPeriodCount { get; set; }
        public CmrrCount EndPeriodCount { get; set; }
    }
}
