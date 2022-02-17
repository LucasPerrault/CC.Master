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
                .Select(dto => dto.ToContract(dto.EnvironmentId.Value))
                .ToList();
        }
    }

    internal class ContractDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Guid ExternalId { get; set; }
        public int? EnvironmentId { get; set; }

        public Contract ToContract(int envId)
        {
            return new Contract
            {
                Id = Id,
                ClientId = ClientId,
                ExternalId = ExternalId,
                EnvironmentId = envId,
            };
        }
    }
}
