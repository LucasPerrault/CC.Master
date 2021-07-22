using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Contracts.Interfaces;
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

        public ContractsStore(ContractsDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<List<Contract>> GetAsync(AccessRight accessRight, ContractFilter filter)
        {
            return await Set()
                .WhereHasRight(accessRight)
                .WhereMatches(filter)
                .ToListAsync();
        }

        private IQueryable<Contract> Set()
        {
            return _dbContext.Set<Contract>()
                .Where(c => !c.ArchivedAt.HasValue || c.ArchivedAt > DateTime.Today);
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
                DistributorCodeAccessRight r => c => c.Distributor.Code == r.DistributorCode,
                DistributorIdAccessRight r => c => c.DistributorId == r.DistributorId,
                AllAccessRight _ => _ => true,
                _ => throw new ApplicationException($"Unknown type of contract filter right {accessRight}")
            };
        }
    }
}
