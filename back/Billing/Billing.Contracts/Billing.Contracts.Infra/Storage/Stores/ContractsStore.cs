using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Contracts.Interfaces;
using Billing.Contracts.Domain.Offers.Services;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Lucca.Core.Shared.Domain.Expressions;
using Microsoft.EntityFrameworkCore;
using Rights.Domain.Filtering;
using Storage.Infra.Extensions;
using Storage.Infra.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tools;

namespace Billing.Contracts.Infra.Storage.Stores
{
    public class ContractsStore : IContractsStore
    {
        private readonly ContractsDbContext _dbContext;
        private readonly IQueryPager _queryPager;

        public ContractsStore(ContractsDbContext dbContext, IQueryPager queryPager)
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
        }

        public Task<Contract> GetSingleAsync(AccessRight accessRight, ContractFilter filter)
        {
            return Set(accessRight, filter)
                .Include(c => c.Environment).ThenInclude(c => c.Establishments).ThenInclude(c => c.Attachments).ThenInclude(c => c.Contract.CommercialOffer)
                .Include(c => c.Environment).ThenInclude(c => c.Establishments).ThenInclude(c => c.Exclusions)
                .SingleOrDefaultAsync();
        }

        public Task<Page<Contract>> GetPageAsync(AccessRight accessRight, ContractFilter filter, IPageToken pageToken)
        {
            return _queryPager.ToPageAsync(Set(accessRight, filter), pageToken);
        }

        public Task<List<Contract>> GetAsync(AccessRight accessRight, ContractFilter filter)
        {
            return Set(accessRight, filter)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<OfferUsageContract>> GetOfferUsageContractAsync(AccessRight accessRight, ContractFilter filter)
        {
            var contractsExtract = await Set(accessRight, filter)
                .AsNoTracking()
                .Select(c => new { c.CommercialOfferId, c.TheoreticalStartOn, c.TheoreticalEndOn})
                .ToListAsync();

            return contractsExtract.Select
            (
                e => new Contract
                {
                    CommercialOfferId = e.CommercialOfferId,
                    TheoreticalStartOn = e.TheoreticalStartOn,
                    TheoreticalEndOn = e.TheoreticalEndOn,
                }
            ).Select
            (
                c => new OfferUsageContract
                {
                    Status = c.Status,
                    CommercialOfferId = c.CommercialOfferId,
                }
            ).ToList();
        }

        public Task<ContractComment> GetCommentAsync(AccessRight accessRight, int contractId)
        {
            return _dbContext.Set<ContractComment>()
                .WhereHasRight(accessRight)
                .Where(c => c.ContractId == contractId)
                .SingleOrDefaultAsync();
        }

        private IQueryable<Contract> Set(AccessRight accessRight, ContractFilter filter)
        {
            return _dbContext.Set<Contract>()
                .Include(c => c.Attachments)
                .Include(c => c.Client)
                .Include(c => c.Distributor)
                .Include(c => c.Environment)
                .Include(c => c.CommercialOffer).ThenInclude(o => o.Product)
                .WhereHasRight(accessRight)
                .WhereMatches(filter);
        }
    }

    internal static class ContractsIQueryableExtensions
    {
        public static IQueryable<Contract> WhereMatches(this IQueryable<Contract> contracts, ContractFilter filter)
        {
            return contracts
                .Search(filter.Search)
                .FilterByStatuses(filter.ContractStatuses)
                .Apply(filter.ArchivedAt).To(c => c.ArchivedAt)
                .Apply(filter.StartsOn).To(ContractExpressions.StartsOn)
                .Apply(filter.EndsOn).To(ContractExpressions.EndsOn)
                .WhenNotNullOrEmpty(filter.ExcludedIds).ApplyWhere(c => !filter.ExcludedIds.Contains(c.Id))
                .Apply(filter.CreatedAt).To(c => c.CreatedAt)
                .Apply(filter.HasEnvironment).To(c => c.EnvironmentId.HasValue)
                .WhenNotNullOrEmpty(filter.CurrentlyAttachedEstablishmentIds).ApplyWhere(ContractExpressions.IsAttachedToAnyEstablishment(filter.CurrentlyAttachedEstablishmentIds, DateTime.Now))
                .WhenNotNullOrEmpty(filter.Ids).ApplyWhere(c => filter.Ids.Contains(c.Id))
                .WhenNotNullOrEmpty(filter.ClientIds).ApplyWhere(c => filter.ClientIds.Contains(c.ClientId))
                .WhenNotNullOrEmpty(filter.ProductIds).ApplyWhere(c => filter.ProductIds.Contains(c.CommercialOffer.ProductId))
                .WhenNotNullOrEmpty(filter.DistributorIds).ApplyWhere(c => filter.DistributorIds.Contains(c.DistributorId))
                .WhenNotNullOrEmpty(filter.ExcludedDistributorIds).ApplyWhere(c => !filter.ExcludedDistributorIds.Contains(c.DistributorId))
                .WhenNotNullOrEmpty(filter.CommercialOfferIds).ApplyWhere(c => filter.CommercialOfferIds.Contains(c.CommercialOfferId))
                .WhenNotNullOrEmpty(filter.EnvironmentIds).ApplyWhere(c => c.EnvironmentId.HasValue &&  filter.EnvironmentIds.Contains(c.EnvironmentId.Value))
                .When(filter.ClientExternalId.HasValue).ApplyWhere(c => c.ClientExternalId == filter.ClientExternalId.Value);
        }

        private static IQueryable<Contract> Search(this IQueryable<Contract> contracts, HashSet<string> words)
        {
            var usableWords = words.Sanitize();
            if (!usableWords.Any())
            {
                return contracts;
            }

            Expression<Func<Contract, bool>> fulltext = c =>
                EF.Functions.Contains(c.Client.Name, usableWords.ToFullTextContainsPredicate())
                || EF.Functions.Contains(c.Client.SocialReason, usableWords.ToFullTextContainsPredicate());

            var startWith = usableWords.Select<string, Expression<Func<Contract, bool>>>
            (
                w => c => c.Environment.Subdomain.StartsWith(w) || c.CommercialOffer.Name.StartsWith(w) || c.Id.ToString() == w
            ).ToArray().CombineSafelyAnd();

            return contracts.Where(fulltext.SmartOrElse(startWith));
        }

        public static IQueryable<Contract> WhereHasRight(this IQueryable<Contract> contracts, AccessRight accessRight)
        {
            return contracts.Where(ToExpression(accessRight));
        }

        public static IQueryable<ContractComment> WhereHasRight(this IQueryable<ContractComment> contracts, AccessRight accessRight)
        {
            return contracts.Where(ToExpressionComment(accessRight));
        }

        private static Expression<Func<Contract, bool>> ToExpression(this AccessRight accessRight)
        {
            return accessRight switch
            {
                NoAccessRight _ => _ => false,
                DistributorAccessRight r => c => c.DistributorId == r.DistributorId,
                AllAccessRight _ => _ => true,
                _ => throw new ApplicationException($"Unknown type of contract filter right {accessRight}")
            };
        }

        private static Expression<Func<ContractComment, bool>> ToExpressionComment(this AccessRight accessRight)
        {
            return accessRight switch
            {
                NoAccessRight _ => _ => false,
                DistributorAccessRight r => c => c.DistributorId == r.DistributorId,
                AllAccessRight _ => _ => true,
                _ => throw new ApplicationException($"Unknown type of contract filter right {accessRight}")
            };
        }

        private static IQueryable<Contract> FilterByStatuses(this IQueryable<Contract> contracts, HashSet<ContractStatus> statuses)
        {
            if (!statuses.Any())
            {
                return contracts;
            }

            if (statuses.ContainsAllEnumValues())
            {
                return contracts;
            }

            var expressions = new List<Expression<Func<Contract, bool>>>();
            if (statuses.Contains(ContractStatus.NotStarted))
            {
                expressions.Add(ContractExpressions.IsNotStarted);
            }

            if (statuses.Contains(ContractStatus.Ended))
            {
                expressions.Add(ContractExpressions.IsEnded);
            }

            if (statuses.Contains(ContractStatus.InProgress))
            {
                expressions.Add(ContractExpressions.IsInProgress);
            }

            return contracts.Where(expressions.ToArray().CombineSafelyOr());
        }
    }
}
