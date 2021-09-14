using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Contracts.Interfaces;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Lucca.Core.Shared.Domain.Expressions;
using Microsoft.EntityFrameworkCore;
using Rights.Domain.Filtering;
using Storage.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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

        public Task<Page<Contract>> GetPageAsync(AccessRight accessRight, ContractFilter filter, IPageToken pageToken)
        {
            return _queryPager.ToPageAsync(Set(accessRight, filter), pageToken);
        }

        public Task<List<Contract>> GetAsync(AccessRight accessRight, ContractFilter filter)
        {
            return Set(accessRight, filter).ToListAsync();
        }

        private IQueryable<Contract> Set(AccessRight accessRight, ContractFilter filter)
        {
            return _dbContext.Set<Contract>()
                .Include(c => c.Attachments)
                .Include(c => c.Client)
                .Include(c => c.Distributor)
                .Include(c => c.CommercialOffer).ThenInclude(o => o.Product)
                .WhereHasRight(accessRight)
                .WhereMatches(filter);
        }
    }

    internal static class ContractsIQueryableExtensions
    {
        public static IQueryable<Contract> WhereMatches(this IQueryable<Contract> contracts, ContractFilter filter)
        {
            return contracts.Search(filter.Search)
                .Apply(filter.Subdomain).To(c => c.EnvironmentSubdomain)
                .Apply(filter.ArchivedAt).To(c => c.ArchivedAt)
                .WhenHasValue(filter.Id).ApplyWhere(c => c.Id == filter.Id.Value)
                .When(filter.ClientExternalId.HasValue).ApplyWhere(c => c.ClientExternalId == filter.ClientExternalId.Value);
        }

        private static IQueryable<Contract> Search(this IQueryable<Contract> contracts, HashSet<string> words)
        {
            if (words == null || !words.Any())
            {
                return contracts;
            }

            Expression<Func<Contract, bool>> fulltext = c =>
                EF.Functions.Contains(c.Client.Name, words.ToFullTextContainsPredicate())
                || EF.Functions.Contains(c.Client.SocialReason, words.ToFullTextContainsPredicate());

            var startWith = words.Select<string, Expression<Func<Contract, bool>>>
            (
                w => c => c.EnvironmentSubdomain.StartsWith(w) || c.CommercialOffer.Name.StartsWith(w) || c.Id.ToString() == w
            ).ToArray().CombineSafelyAnd();

            return contracts.Where(fulltext.SmartOrElse(startWith));
        }

        public static IQueryable<Contract> WhereHasRight(this IQueryable<Contract> contracts, AccessRight accessRight)
        {
            return contracts.Where(ToExpression(accessRight));
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
    }
}
