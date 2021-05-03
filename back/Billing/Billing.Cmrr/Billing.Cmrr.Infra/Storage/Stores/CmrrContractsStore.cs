using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public Task<List<CmrrContract>> GetContractsNotEndedAtAsync(DateTime startPeriod, DateTime endPeriod)
        {
            return NotArchivedContract.Where(c => !c.EndDate.HasValue || c.EndDate >= startPeriod)
                                      .Where(c => c.StartDate <= endPeriod)
                                      .ToListAsync();
        }

        private IQueryable<CmrrContract> NotArchivedContract => _dbContext.Set<CmrrContract>().Where(c => !c.IsArchived);
    }
}
