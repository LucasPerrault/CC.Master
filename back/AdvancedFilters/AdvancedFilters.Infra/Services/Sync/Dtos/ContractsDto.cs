using AdvancedFilters.Domain.Billing.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedFilters.Infra.Services.Sync.Dtos
{
    internal class ContractsDto : IDto<Contract>
    {
        public List<ContractDto> Items { get; set; }

        public List<Contract> ToItems()
        {
            return Items
                .Select(dto => dto.ToContract())
                .ToList();
        }
    }

    internal class ContractDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Guid ExternalId { get; set; }
        public IReadOnlyCollection<EstablishmentAttachmentDto> EstablishmentAttachments { get; set; }

        public Contract ToContract()
        {
            return new Contract
            {
                Id = Id,
                ClientId = ClientId,
                ExternalId = ExternalId,
                EstablishmentAttachments = GetFromDto(EstablishmentAttachments)
            };
        }

        private IReadOnlyCollection<EstablishmentContract> GetFromDto(IReadOnlyCollection<EstablishmentAttachmentDto> establishmentAttachments)
        {
            return establishmentAttachments
                .Where(ea => ea.IsActive)
                .Select(ea => ea.ToEstablishmentContract(Id))
                .ToList();
        }
    }

    internal class EstablishmentAttachmentDto
    {
        public int EstablishmentId { get; set; }
        public DateTime StartsOn { get; set; }
        public DateTime? EndsOn { get; set; }

        public bool IsActive
        {
            get
            {
                var hasStarted = DateTime.Now > StartsOn;
                var hasNotEnded = !EndsOn.HasValue || DateTime.Now < EndsOn.Value;
                return hasStarted && hasNotEnded;
            }
        }

        public EstablishmentContract ToEstablishmentContract(int contractId)
        {
            return new EstablishmentContract
            {
                ContractId = contractId,
                EstablishmentId = EstablishmentId
            };
        }
    }
}
