using Billing.Contracts.Domain.Clients.Interfaces;
using Billing.Contracts.Domain.Environments;
using Microsoft.EntityFrameworkCore;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Contracts.Infra.Storage.Stores
{
    public class ContractEnvironmentStore : IContractEnvironmentStore
    {
        private readonly ContractsDbContext _dbContext;

        public ContractEnvironmentStore(ContractsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<ContractEnvironment>> GetAsync(AccessRight accessRight, HashSet<int> environmentIds)
        {

            return _dbContext.Set<ContractEnvironment>()
                .Include(e => e.Establishments).ThenInclude(e => e.Attachments)
                .Include(e => e.Establishments).ThenInclude(e => e.Exclusions)
                .WhereHasAccess(accessRight)
                .Where(e => environmentIds.Contains(e.Id))
                .ToListAsync();
        }
    }

    public static class ContractEnvironmentQueryableExtensions
    {
        public static IQueryable<ContractEnvironment> WhereHasAccess(this IQueryable<ContractEnvironment> environments, AccessRight accessRight)
        {
            return accessRight switch
            {
                AllAccessRight _ => environments,
                _ => throw new ApplicationException("Contract environment store cannot be access with partial rights.")
            };
        }
    }
}
