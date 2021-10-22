using Billing.Contracts.Application;
using Billing.Contracts.Application.Clients;
using Billing.Contracts.Domain.Clients;
using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Contracts.Health;
using Billing.Contracts.Domain.Environments;
using Billing.Products.Domain;
using Distributors.Domain.Models;
using IdentityModel.Client;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<Page<ContractDto>> GetAsync(IPageToken pageToken, [FromQuery]ContractListQuery query)
        {
            var page = await _contractsRepository.GetPageAsync(pageToken, query.ToFilter());
            return new Page<ContractDto>
            {
                Count = page.Count,
                Next = page.Next,
                Prev = page.Prev,
                Items = page.Items.Select(p => new ContractDto(p))
            };
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

        [HttpGet("{id:int}")]
        [ForbidIfMissing(Operation.ReadContracts)]
        public async Task<ContractDto> GetAsync([FromRoute]int id)
        {
            var contracts = await _contractsRepository.GetAsync(new ContractFilter { Ids = new HashSet<int>{ id }, ArchivedAt = ContractListQuery.NotArchived()});
            if (!contracts.Any())
            {
                throw new NotFoundException();
            }

            return new ContractDto(contracts.Single());
        }

        [HttpGet("{id:int}/comment")]
        [ForbidIfMissing(Operation.ReadContracts)]
        public Task<ContractComment> GetCommentAsync([FromRoute] int id)
        {
            return _contractsRepository.GetCommentAsync(id);
        }

        public class ContractDto
        {
            public int Id { get; set; }
            public Guid ExternalId { get; set; }
            public Guid ClientExternalId { get; set; }
            public int? EnvironmentId { get; set; }
            public ContractEnvironment Environment { get; set; }
            public DateTime CreatedAt { get; set; }

            public DateTime TheoreticalStartOn { get; set; }
            public DateTime? TheoreticalEndOn { get; set; }
            public ContractEndReason EndReason { get; set; }

            public DateTime? ArchivedAt { get; set; }

            public int DistributorId { get; set; }
            public Distributor Distributor { get; set; }

            public int ClientId { get; set; }
            public ContractDtoClient Client { get; set; }


            public int CommercialOfferId { get; set; }
            public CommercialOffer CommercialOffer { get; set; }

            public List<EstablishmentAttachment> Attachments { get; set; }

            public decimal CountEstimation { get; set; }
            public int TheoreticalFreeMonths { get; set; }
            public double? RebatePercentage { get; set; }
            public DateTime? RebateEndsOn { get; set; }
            public double MinimalBillingPercentage { get; set; }
            public BillingPeriodicity BillingPeriodicity { get; set; }

            public ContractDto(Contract contract)
            {
                Id = contract.Id;
                ExternalId = contract.ExternalId;
                ClientExternalId = contract.ClientExternalId;
                EnvironmentId = contract.EnvironmentId;
                Environment = contract.Environment;
                TheoreticalStartOn = contract.TheoreticalStartOn;
                TheoreticalEndOn = contract.TheoreticalEndOn;
                CreatedAt = contract.CreatedAt;
                ArchivedAt = contract.ArchivedAt;
                DistributorId = contract.DistributorId;
                Distributor = contract.Distributor;
                ClientId = contract.ClientId;
                Client = new ContractDtoClient(contract.Client);
                CommercialOfferId = contract.CommercialOfferId;
                CommercialOffer = contract.CommercialOffer;
                Attachments = contract.Attachments;
                CountEstimation = contract.CountEstimation;
                TheoreticalFreeMonths = contract.TheoreticalFreeMonths;
                RebatePercentage = contract.RebatePercentage;
                BillingPeriodicity = contract.BillingPeriodicity;
                RebateEndsOn = contract.RebateEndsOn;
                MinimalBillingPercentage = contract.MinimalBillingPercentage;
            }
        }

        public class ContractDtoClient
        {
            public int Id { get; set; }
            public Guid ExternalId { get; set; }
            public string Name { get; set; }
            public string SocialReason { get; set; }
            public string SalesforceId { get; set; }

            public ContractDtoClient(Client contractClient)
            {
                Id = contractClient.Id;
                ExternalId = contractClient.ExternalId;
                Name = contractClient.Name;
                SocialReason = contractClient.SocialReason;
                SalesforceId = contractClient.SalesforceId;
            }
        }
    }

    public class ContractListQuery
    {
        public static CompareNullableDateTime NotArchived() => CompareDateTime.IsStrictlyAfter(DateTime.Now).OrNull();
        public HashSet<int> Id { get; set; } = new HashSet<int>();
        public HashSet<string> Search { get; set; } = new HashSet<string>();
        public HashSet<int> EnvironmentId { get; set; } = new HashSet<int>();
        public DateTime? WasStartedOn { get; set; } = null;
        public DateTime? WasNotEndedOn { get; set; } = null;

        public ContractFilter ToFilter() => new ContractFilter
        {
            Search = Search,
            EnvironmentIds = EnvironmentId,
            Ids = Id,
            ClientIds = ClientId,
            DistributorIds = DistributorId,
            CommercialOfferIds = CommercialOfferId,
            ExcludedDistributorIds = ExcludedDistributorId,
            ProductIds = ProductId,
            ArchivedAt = NotArchived(),
            StartsOn = WasStartedOn.HasValue ? CompareDateTime.IsBeforeOrEqual(WasStartedOn.Value) : CompareDateTime.Bypass(),
            EndsOn = WasNotEndedOn.HasValue ? CompareDateTime.IsAfterOrEqual(WasNotEndedOn.Value).OrNull() : CompareNullableDateTime.Bypass()
        };

        public HashSet<int> ClientId { get; set; } = new HashSet<int>();
        public HashSet<int> DistributorId { get; set; } = new HashSet<int>();
        public HashSet<int> ExcludedDistributorId { get; set; } = new HashSet<int>();
        public HashSet<int> ProductId { get; set; } = new HashSet<int>();
        public HashSet<int> CommercialOfferId { get; set; } = new HashSet<int>();
    }
}
