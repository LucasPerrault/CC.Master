using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Billing.Cmrr.Infra.Storage.Stores
{
    public class CmrrContractsStore : ICmrrContractsStore
    {
        private readonly CmrrDbContext _dbContext;

        public CmrrContractsStore(CmrrDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task<List<CmrrContract>> GetContractsNotEndedAtAsync(DateTime startPeriod, DateTime endPeriod, AccessRight accessRight)
        {
            return NotArchivedContract.WhereHasRight(accessRight)
                                      .Where(c => !c.EndDate.HasValue || c.EndDate >= startPeriod)
                                      .Where(c => c.StartDate <= endPeriod)
                                      .ToListAsync();
        }

        private IQueryable<CmrrContract> NotArchivedContract => _dbContext
            .Set<CmrrContract>()
            .Include(c => c.Distributor)
            .Where(c => !c.IsArchived)
            .AsNoTracking();
    }

    internal static class CmrrContractQueryableExtensions
    {
        public static IQueryable<CmrrContract> WhereHasRight(this IQueryable<CmrrContract> query, AccessRight accessRight)
        {
            return query.Where(ToRightExpression(accessRight));
        }

        private static Expression<Func<CmrrContract, bool>> ToRightExpression(AccessRight access)
        {
            return access switch
            {
                NoAccessRight _ => _ => false,
                DistributorAccessRight r => c=> c.DistributorId == r.DistributorId,
                AllAccessRight _ => _ => true,
                _ => throw new ApplicationException($"Unknown type of demo filter right {access}")
            };
        }
    }
}
