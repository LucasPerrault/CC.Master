using Billing.Contracts.Domain.Clients;
using Billing.Contracts.Domain.Clients.Interfaces;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
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
    public class ClientsStore : IClientsStore
    {
        private readonly ContractsDbContext _dbContext;
        private readonly IQueryPager _queryPager;

        public ClientsStore(ContractsDbContext dbContext, IQueryPager queryPager)
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
        }

        public Task<List<Client>> GetAsync(AccessRight accessRight, ClientFilter filter)
        {
            return GetQueryable(accessRight, filter).ToListAsync();
        }

        public async Task<Page<Client>> GetPageAsync(IPageToken pageToken, AccessRight accessRight, ClientFilter filter)
        {
            return await _queryPager.ToPageAsync(GetQueryable(accessRight, filter), pageToken);
        }

        private IQueryable<Client> GetQueryable(AccessRight accessRight, ClientFilter filter)
        {
            return _dbContext.Set<Client>()
                .WhereHasRight(accessRight)
                .WhereMatches(filter);
        }
    }

    internal static class ClientsQueryableExtensions
    {
        public static IQueryable<Client> WhereMatches(this IQueryable<Client> clients, ClientFilter filter)
        {
            return clients
                .When(filter.ExternalId.HasValue).ApplyWhere(c => c.ExternalId == filter.ExternalId.Value);
        }

        public static IQueryable<Client> WhereHasRight(this IQueryable<Client> clients, AccessRight accessRight)
        {
            return clients.Where(accessRight.ToRightExpression());
        }

        private static Expression<Func<Client, bool>> ToRightExpression(this AccessRight accessRight)
        {
            return accessRight switch
            {
                NoAccessRight _ => _ => false,
                DistributorAccessRight r => c => c.Contracts.Any(c => c.DistributorId == r.DistributorId),
                EnvironmentAccessRight r => c => c.Contracts.Any(c => c.EnvironmentSubdomain == r.Subdomain),
                AllAccessRight _ => _ => true,
                _ => throw new ApplicationException($"Unknown type of contract filter right {accessRight}")
            };
        }
    }
}
