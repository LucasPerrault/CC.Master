using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Contracts.Interfaces;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Microsoft.EntityFrameworkCore;
using Rights.Domain.Filtering;
using Storage.Infra.Extensions;
using System;
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

        private IQueryable<Contract> Set(AccessRight accessRight, ContractFilter filter)
        {
            return _dbContext.Set<Contract>()
                .Where(c => !c.ArchivedAt.HasValue || c.ArchivedAt > DateTime.Today)
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
            return contracts
                .Apply(filter.Subdomain).To(c => c.EnvironmentSubdomain)
                .When(filter.ClientExternalId.HasValue).ApplyWhere(c => c.ClientExternalId == filter.ClientExternalId.Value);
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
