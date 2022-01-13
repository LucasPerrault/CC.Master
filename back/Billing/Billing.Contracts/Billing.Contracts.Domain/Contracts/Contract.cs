using Billing.Contracts.Domain.Clients;
using Billing.Contracts.Domain.Distributors;
using Billing.Contracts.Domain.Environments;
using Billing.Contracts.Domain.Offers;
using Distributors.Domain.Models;
using Lucca.Core.Shared.Domain.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tools;

namespace Billing.Contracts.Domain.Contracts
{
    public class Contract
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public Guid ClientExternalId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime TheoreticalStartOn { get; set; }
        public DateTime? TheoreticalEndOn { get; set; }
        public ContractEndReason EndReason { get; set; }

        public DateTime StartsOn => ContractExpressions.StartsOnCompiled(this);
        public DateTime? EndsOn => ContractExpressions.EndsOnCompiled(this);

        public DateTime? ArchivedAt { get; set; }

        public int DistributorId { get; set; }
        public Distributor Distributor { get; set; }
        public DistributorBillingPreference DistributorBillingPreference { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }

        public int? EnvironmentId { get; set; }
        public ContractEnvironment Environment { get; set; }

        public int CommercialOfferId { get; set; }
        public CommercialOffer CommercialOffer { get; set; }

        public List<EstablishmentAttachment> Attachments { get; set; }

        public int CountEstimation { get; set; }
        public int TheoreticalFreeMonths { get; set; }
        public double? RebatePercentage { get; set; }
        public DateTime? RebateEndsOn { get; set; }
        public double MinimalBillingPercentage { get; set; }
        public BillingPeriodicity BillingPeriodicity { get; set; }

        public ContractStatus Status => ContractExpressions.IsNotStartedCompiled(this)
            ? ContractStatus.NotStarted
            : ContractExpressions.IsEndedCompiled(this)
                ? ContractStatus.Ended
                : ContractStatus.InProgress;
    }

    public static class ContractExpressions
    {
        public static readonly Expression<Func<Contract, DateTime>> StartsOn = c => c.Attachments.Any()
            ? c.Attachments
                .Select(a => a.StartsOn)
                .DefaultIfEmpty()
                .Min()
            : c.TheoreticalStartOn;
        public static readonly Func<Contract, DateTime> StartsOnCompiled = StartsOn.Compile();

        public static readonly Expression<Func<Contract, DateTime?>> EndsOn = c =>
            c.Attachments.Any() && c.Attachments.All(a => a.EndsOn.HasValue)
                ? c.Attachments
                    .Select(a => a.EndsOn)
                    .DefaultIfEmpty()
                    .OrderByDescending(endsOn => endsOn)
                    .Max()
                : c.TheoreticalEndOn;
        public static readonly Func<Contract, DateTime?> EndsOnCompiled = EndsOn.Compile();

        public static Expression<Func<Contract, bool>> IsAttachedToAnyEstablishment(HashSet<int> establishmentIds, DateTime period) => c =>
        c.Attachments.Any(a =>
            establishmentIds.Contains(a.EstablishmentId)
            && a.StartsOn < period
            && ( a.EndsOn == null || a.EndsOn > period )
        );

        public static readonly Expression<Func<Contract, bool>> IsNotStarted = c => c.TheoreticalStartOn > DateTime.Today;
        public static readonly Func<Contract, bool> IsNotStartedCompiled = IsNotStarted.Compile();

        public static readonly Expression<Func<Contract, bool>> IsEnded = c => c.TheoreticalEndOn < DateTime.Today;
        public static readonly Func<Contract, bool> IsEndedCompiled = IsEnded.Compile();

        public static Expression<Func<Contract, bool>> IsInProgress =>
            IsNotStarted.Inverse().SmartAndAlso(
                IsEnded.Inverse().SmartOrElse(c => !c.TheoreticalEndOn.HasValue)
            );
    }

    public enum ContractEndReason
    {
        Unknown = 0,
        Modification = 1,
        Resiliation = 2,
        Deactivation = 3,
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
}
