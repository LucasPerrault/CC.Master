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
                .Where(dto => dto.EnvironmentId.HasValue)
                .Select(dto => dto.ToContract())
                .ToList();
        }
    }

    internal class ContractDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Guid ExternalId { get; set; }
        public int? EnvironmentId { get; set; }
        public IReadOnlyCollection<EstablishmentAttachmentDto> Attachments { get; set; }

        public Contract ToContract()
        {
            return new Contract
            {
                Id = Id,
                ClientId = ClientId,
                ExternalId = ExternalId,
                EstablishmentAttachments = GetFromDto(Attachments)
            };
        }

        private IReadOnlyCollection<EstablishmentContract> GetFromDto(IReadOnlyCollection<EstablishmentAttachmentDto> establishmentAttachments)
        {
            return establishmentAttachments
                .Where(ea => ea.IsActive)
                .Select(ea => ea.ToEstablishmentContract(Id, EnvironmentId.Value))
                .ToList();
        }
    }

    internal class EstablishmentAttachmentDto
    {
        public int EstablishmentRemoteId { get; set; }
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

        public EstablishmentContract ToEstablishmentContract(int contractId, int environmentId)
        {
            return new EstablishmentContract
            {
                ContractId = contractId,
                EnvironmentId = environmentId,
                EstablishmentId = EstablishmentRemoteId
            };
        }
    }
}
