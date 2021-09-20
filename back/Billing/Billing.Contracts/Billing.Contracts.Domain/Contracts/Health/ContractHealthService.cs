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
                .Where(h => h.EstablishmentHealths.Any())
                .ToList();
        }

        private ContractHealth GetContractHealth(Contract contract, ContractEnvironment environment)
        {
            return new ContractHealth
            {
                ContractId = contract.Id,
                EnvironmentId = environment.Id,
                EstablishmentHealths = environment.Establishments
                    .Select(e => GetEstablishmentHealth(e, contract.CommercialOffer.ProductId))
                    .Where(e => e.AttachmentErrors.Any())
                    .ToList()
            };
        }

        private EstablishmentHealth GetEstablishmentHealth(Establishment establishment, int productId)
        {
            return new EstablishmentHealth
            {
                EstablishmentId = establishment.Id,
                AttachmentErrors = GetAttachmentErrors(establishment, productId),
            };
        }

        private List<ContractAttachmentError> GetAttachmentErrors(Establishment establishment, int productId)
        {
            if (IsExcluded(establishment, productId))
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

        private bool IsExcluded(Establishment establishment, int productId)
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
