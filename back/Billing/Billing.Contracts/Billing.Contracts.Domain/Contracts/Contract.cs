using Billing.Contracts.Domain.Clients;
using Billing.Products.Domain;
using Distributors.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Billing.Contracts.Domain.Contracts
{
    public class Contract
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public Guid ClientExternalId { get; set; }
        public int? EnvironmentId { get; set; }
        public string EnvironmentSubdomain { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime TheoreticalStartOn { get; set; }
        public DateTime? TheoreticalEndOn { get; set; }
        public ContractEndReason EndReason { get; set; }

        public DateTime? ArchivedAt { get; set; }

        public int DistributorId { get; set; }
        public Distributor Distributor { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; }


        public int CommercialOfferId { get; set; }
        public CommercialOffer CommercialOffer { get; set; }

        public List<EstablishmentAttachment> Attachments { get; set; }

        public decimal CountEstimation { get; set; }
        public int TheoreticalFreeMonths { get; set; }
        public double? RebatePercentage { get; set; }
        public DateTime? RebateEndsOn { get; set; }
        public double MinimalBillingPercentage { get; set; }
        public BillingPeriodicity BillingPeriodicity { get; set; }
    }

    public static class ContractExpressions
    {
        public static readonly Expression<Func<Contract, DateTime>> StartsOn = c => c.Attachments.Any()
            ? c.Attachments
                .Where(a => a.IsActive)
                .Select(a => a.StartsOn)
                .Min()
            : c.TheoreticalStartOn;

        public static readonly Expression<Func<Contract, DateTime?>> EndsOn = c => c.Attachments.Any()
            ? c.Attachments
                .Where(a => a.IsActive)
                .Select(a => a.EndsOn)
                .Min()
            : null;
    }

    public enum ContractEndReason
    {
        Unknown = 0,
        Modification = 1,
        Resiliation = 2,
        Deactivation = 3
    }

    public enum BillingPeriodicity
    {
        Unknown = 0,
        AnnualJanuary = 1,
        AnnualFebruary = 2,
        AnnualMarch = 3,
        AnnualApril = 4,
        AnnualMay = 5,
        AnnualJune = 6,
        AnnualJuly = 7,
        AnnualAugust = 8,
        AnnualSeptember = 9,
        AnnualOctober = 10,
        AnnualNovember = 11,
        AnnualDecember = 12,
        Quarterly = 13,
    }

    public class EstablishmentAttachment
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int EstablishmentId { get; set; }
        public int EstablishmentRemoteId { get; set; }
        public string EstablishmentName { get; set; }
        public DateTime StartsOn { get; set; }
        public DateTime? EndsOn { get; set; }
        public bool IsActive { get; set; }
    }
}
