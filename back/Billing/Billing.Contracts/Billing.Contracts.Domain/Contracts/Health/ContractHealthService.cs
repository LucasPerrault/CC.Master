using Billing.Contracts.Domain.Environments;
using System.Collections.Generic;

namespace Billing.Contracts.Domain.Contracts.Health
{
    public class ContractHealthService
    {
        public List<ContractHealth> GetHealth(ContractEnvironment environment, List<Contract> contracts)
        {
            //TODO implement environment health rules
            return new List<ContractHealth>
            {
                new ContractHealth
                {
                    EnvironmentId = environment.Id,
                    EstablishmentHealths = new List<EstablishmentHealth>
                    {
                        new EstablishmentHealth
                        {
                            EstablishmentId = 0,
                            AttachmentErrors = new List<ContractAttachmentError>
                            {
                                ContractAttachmentError.MissingAttachment,
                                ContractAttachmentError.InactiveAttachment
                            }
                        }
                    }
                }
            };
        }
    }
}
