using Billing.Contracts.Domain.Contracts;
using Billing.Contracts.Domain.Contracts.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IReadOnlyCollection<Contract>> GetAsync(Guid clientExternalId, string subdomain)
        {
            return await Set()
                .Where(contract =>
                        contract.ClientExternalId == clientExternalId
                        && contract.EnvironmentSubdomain == subdomain
                    )
                .ToListAsync();
        }

        private IQueryable<Contract> Set()
        {
            return _dbContext.Set<Contract>()
                .Where(c => !c.ArchivedAt.HasValue || c.ArchivedAt > DateTime.Today);
        }
    }
}
