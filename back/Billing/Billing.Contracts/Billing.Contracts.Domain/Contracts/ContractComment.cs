namespace Billing.Contracts.Domain.Contracts
{
    public class ContractComment
    {
        public int ContractId { get; set; }
        public string Comment { get; set; }
        public int DistributorId { get; set; }
    }
}
