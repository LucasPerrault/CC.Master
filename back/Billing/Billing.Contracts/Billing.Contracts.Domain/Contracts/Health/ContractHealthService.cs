using System;
using Billing.Contracts.Domain.Environments;
using System.Collections.Generic;
using System.Linq;

namespace Billing.Contracts.Domain.Contracts.Health
{
    public class ContractHealthService
    {
        public List<ContractHealth> GetHealth(ContractEnvironment environment, List<Contract> contracts)
        {
            return contracts
                .Select(c => GetContractHealth(c, environment))
                .Where(h => h.Establishments.Any())
                .ToList();
        }

        private ContractHealth GetContractHealth(Contract contract, ContractEnvironment environment)
        {
            return new ContractHealth
            {
                ContractId = contract.Id,
                EnvironmentId = environment.Id,
                Establishments = environment.Establishments
                    .Select(e => GetEstablishmentHealth(contract.CommercialOffer.ProductId, e))
                    .Where(e => e.AttachmentErrors.Any())
                    .ToList()
            };
        }

        private EstablishmentHealth GetEstablishmentHealth(int productId, Establishment establishment)
        {
            return new EstablishmentHealth
            {
                Id = establishment.Id,
                AttachmentErrors = GetAttachmentErrors(productId, establishment),
            };
        }

        private List<ContractAttachmentError> GetAttachmentErrors(int productId, Establishment establishment)
        {
            if (IsExcluded(productId, establishment))
            {
                return new List<ContractAttachmentError>();
            }

            if (!HasActiveAttachments(establishment))
            {
                return new List<ContractAttachmentError>() { ContractAttachmentError.MissingAttachment };
            }

            if (!establishment.IsActive)
            {
                return new List<ContractAttachmentError>() { ContractAttachmentError.InactiveAttachment };
            }

            return new List<ContractAttachmentError>();
        }

        private bool IsExcluded(int productId, Establishment establishment)
        {
            return establishment.Exclusions.Any(e => e.ProductId == productId);
        }

        private bool HasActiveAttachments(Establishment establishment)
        {
            return establishment.Attachments.Any(a => !IsFinished(a));
        }

        private bool IsFinished(EstablishmentAttachment attachment)
        {
            if (!attachment.EndsOn.HasValue)
            {
                return false;
            }

            return attachment.EndsOn > DateTime.Today;
        }
    }
}
