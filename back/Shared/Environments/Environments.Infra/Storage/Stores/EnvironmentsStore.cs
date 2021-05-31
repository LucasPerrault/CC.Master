using Environments.Domain.Storage;
using Microsoft.EntityFrameworkCore;
using Rights.Domain.Filtering;
using Storage.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Environment = Environments.Domain.Environment;

namespace Environments.Infra.Storage.Stores
{
    public class EnvironmentsStore : IEnvironmentsStore
    {
        private readonly EnvironmentsDbContext _dbContext;

        public EnvironmentsStore(EnvironmentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Environment>> GetAsync(AccessRight accessRight, PurposeAccessRight purposeAccessRight, EnvironmentFilter filter)
        {
            return _dbContext.Set<Environment>()
                .ForRights(accessRight)
                .WithPurposes(purposeAccessRight)
                .FilterBy(filter)
                .ToListAsync();
        }
    }

    internal static class EnvironmentQueryableExtensions
    {
        public static IQueryable<Environment> FilterBy(this IQueryable<Environment> environments, EnvironmentFilter filter)
        {
            return environments
                .WhereStringCompares(filter.Subdomain, e => e.Subdomain)
                .Apply(filter.IsActive).To(e => e.IsActive);
        }
        public static IQueryable<Environment> ForRights(this IQueryable<Environment> environments, AccessRight accessRight)
        {
            return accessRight switch
            {
                NoAccessRight _ => new List<Environment>().AsQueryable(),
                DistributorCodeAccessRight r => environments.Where(e => e.ActiveAccesses.Any(a => a.ConsumerId == r.DistributorCode)),
                AllAccessRight _ => environments,
                _ => throw new ApplicationException($"Unknown type of access right {accessRight}")
            };
        }

        public static IQueryable<Environment> WithPurposes(this IQueryable<Environment> environments, PurposeAccessRight accessRight)
        {
            return accessRight switch
            {
                AllPurposeAccessRight _ => environments,
                SomePurposesAccessRight r => environments.Where(e => r.Purposes.Contains(e.Purpose)),
                _ => throw new ApplicationException($"Unknown type of purpose access right {accessRight}")
            };
        }
    }
}
