using Billing.Contracts.Application;
using Billing.Contracts.Application.Clients;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Contracts.Health;
using Billing.Contracts.Domain.Environments;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using NExtends.Primitives.DateTimes;
using Rights.Domain;
using Rights.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools;

namespace Billing.Contracts.Web
{
    [ApiController, Route("/api/contracts")]
    [ApiSort(nameof(Contract.Id))]
    [DefaultLimit(25)]
    public class ContractsController
    {
        private readonly ContractsRepository _contractsRepository;
        private readonly ContractHealthHelper _healthHelper;

        public ContractsController(ContractsRepository contractsRepository, ContractHealthHelper healthHelper)
        {
            _contractsRepository = contractsRepository;
            _healthHelper = healthHelper;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.ReadContracts)]
        public Task<Page<Contract>> GetAsync(IPageToken pageToken, [FromQuery]ContractListQuery query)
        {
            return _contractsRepository.GetPageAsync(pageToken, query.ToFilter());
        }

        [HttpGet("health")]
        [ForbidIfMissing(Operation.ReadContracts)]
        public async Task<Page<ContractHealth>> GetHealthAsync([FromQuery]ContractListQuery query)
        {
            var healths = await _healthHelper.GetHealthAsync(query.ToFilter());
            return new Page<ContractHealth>
            {
                Items = healths,
                Count = healths.Count
            };
        }

        [HttpGet("{id:int}/environment")]
        [ForbidIfMissing(Operation.ReadContracts)]
        public Task<ContractEnvironment> GetEnvironmentAsync([FromRoute]int id)
        {
            return _contractsRepository.GetEnvironmentAsync(id);
        }

        [HttpGet("{id:int}")]
        [ForbidIfMissing(Operation.ReadContracts)]
        public async Task<Contract> GetAsync([FromRoute]int id)
        {
            var contracts = await _contractsRepository.GetAsync(new ContractFilter { Ids = new HashSet<int>{ id }, ArchivedAt = ContractListQuery.NotArchived()});
            if (!contracts.Any())
            {
                throw new NotFoundException();
            }

            return contracts.Single();
        }

        [HttpGet("{id:int}/comment")]
        [ForbidIfMissing(Operation.ReadContracts)]
        public Task<ContractComment> GetCommentAsync([FromRoute] int id)
        {
            return _contractsRepository.GetCommentAsync(id);
        }
    }

    public class ContractListQuery
    {
        public static CompareNullableDateTime NotArchived() => CompareDateTime.IsStrictlyAfter(DateTime.Now).OrNull();
        public HashSet<int> Id { get; set; } = new HashSet<int>();
        public HashSet<string> Search { get; set; } = new HashSet<string>();
        public HashSet<int> EnvironmentId { get; set; } = new HashSet<int>();
        public DateTime? WasStartedOn { get; set; } = null;
        public DateTime? TheoreticalStartOn { get; set; } = null;
        public DateTime? WasNotEndedOn { get; set; } = null;
        public DateTime? TheoreticalEndOn { get; set; } = null;
        public DateTime? CreationMonth { get; set; } = null;

        public ContractFilter ToFilter() => new ContractFilter
        {
            Search = Search,
            EnvironmentIds = EnvironmentId,
            Ids = Id,
            ClientIds = ClientId,
            DistributorIds = DistributorId,
            CommercialOfferIds = CommercialOfferId,
            ExcludedDistributorIds = ExcludedDistributorId,
            CurrentlyAttachedEstablishmentIds = CurrentlyAttachedEstablishmentId,
            ProductIds = ProductId,
            ContractStatuses = Status,
            ArchivedAt = NotArchived(),
            CreatedAt = GetCreatedAt(),
            StartsOn = GetStartsOn(),
            EndsOn = GetEndsOn()
        };

        private CompareDateTime GetCreatedAt()
        {
            return CreationMonth.HasValue
                ? CompareDateTime.IsBetweenOrEqual(CreationMonth.Value.FirstOfMonth(), CreationMonth.Value.AddMonths(1).FirstOfMonth())
                : CompareDateTime.Bypass();
        }

        private CompareDateTime GetStartsOn()
        {
            if (!TheoreticalStartOn.HasValue && !WasStartedOn.HasValue)
            {
                return CompareDateTime.Bypass();
            }
            if (!WasStartedOn.HasValue)
            {
                return CompareDateTime.IsEqual(TheoreticalStartOn.Value);
            }
            if (!TheoreticalStartOn.HasValue)
            {
                return CompareDateTime.IsBeforeOrEqual(WasStartedOn.Value);
            }
            if (WasStartedOn >= TheoreticalStartOn)
            {
                return CompareDateTime.IsBeforeOrEqual(WasStartedOn.Value);
            }
            return CompareDateTime.MatchesNone();
        }

        private CompareNullableDateTime GetEndsOn()
        {
            if (!TheoreticalEndOn.HasValue && !WasNotEndedOn.HasValue)
            {
                return CompareNullableDateTime.Bypass();
            }
            if (!WasNotEndedOn.HasValue)
            {
                return CompareDateTime.IsEqual(TheoreticalEndOn.Value).AndNotNull();
            }
            if (!TheoreticalEndOn.HasValue)
            {
                return CompareDateTime.IsBeforeOrEqual(WasNotEndedOn.Value).OrNull();
            }
            if (WasNotEndedOn <= TheoreticalEndOn)
            {
                return CompareDateTime.IsEqual(TheoreticalEndOn.Value).AndNotNull();
            }
            return CompareNullableDateTime.MatchesNone();
        }

        public HashSet<ContractStatus> Status { get; set; } = new HashSet<ContractStatus>();
        public HashSet<int> CurrentlyAttachedEstablishmentId { get; set; } = new HashSet<int>();
        public HashSet<int> ClientId { get; set; } = new HashSet<int>();
        public HashSet<int> DistributorId { get; set; } = new HashSet<int>();
        public HashSet<int> ExcludedDistributorId { get; set; } = new HashSet<int>();
        public HashSet<int> ProductId { get; set; } = new HashSet<int>();
        public HashSet<int> CommercialOfferId { get; set; } = new HashSet<int>();
    }
}
