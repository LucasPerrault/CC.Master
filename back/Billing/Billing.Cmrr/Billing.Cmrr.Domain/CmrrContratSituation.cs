namespace Billing.Cmrr.Domain
{
    public class CmrrContratSituation
    {
        public int ContractId { get; set; }
        public CmrrContract Contract { get; set; }

        public CmrrCount FirstPeriodCount { get; set; }
        public CmrrCount LastPeriodCount { get; set; }
    }
}
