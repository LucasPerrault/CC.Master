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
                RemoteId = Id,
                ClientId = ClientId,
                ExternalId = ExternalId,
                EstablishmentAttachments = EstablishmentAttachments // TODO where active (DateTime.Now entre StartAt et EndsAt)
                    .Select(ea => ea.ToEstablishmentContract(Id))
                    .ToList()
            };
        }
    }

    internal class EstablishmentAttachmentDto
    {
        public int EstablishmentId { get; set; }

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
