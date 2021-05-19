using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Cmrr.Infra.Storage.Stores
{
    public class CmrrCountsStore : ICmrrCountsStore
    {
        private readonly CmrrDbContext _dbContext;

        public CmrrCountsStore(CmrrDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task<List<CmrrCount>> GetByPeriodAsync(DateTime period)
        {
            return _dbContext.Set<CmrrCount>()
                .Where(c => c.CountPeriod == period)
                .ToListAsync();
        }
    }
}
