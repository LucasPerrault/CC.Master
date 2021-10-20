using System.Collections.Generic;

namespace Billing.Contracts.Domain.Contracts.Health
{

    public class ContractHealth
    {
        public int ContractId { get; set; }
        public int EnvironmentId { get; set; }
        public List<EstablishmentHealth> Establishments { get; set; }
    }

    public class EstablishmentHealth
    {
        public int Id { get; set; }
        public List<ContractAttachmentError> AttachmentErrors { get; set; }
    }

    public enum ContractAttachmentError
    {
        Unknown = 0,
        MissingAttachment = 1,
        InactiveAttachment = 2
    }
}
