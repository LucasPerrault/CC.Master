using Billing.Contracts.Domain.Clients;
using Billing.Contracts.Domain.Clients.Interfaces;
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

        public ClientsStore(ContractsDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task<List<Client>> GetAsync(AccessRight accessRight, ClientFilter filter)
        {
            return _dbContext.Set<Client>()
                .WhereHasRight(accessRight)
                .WhereMatches(filter)
                .ToListAsync();
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
