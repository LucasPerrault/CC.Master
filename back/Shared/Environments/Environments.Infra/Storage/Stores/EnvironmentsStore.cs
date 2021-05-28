using Environments.Domain.Storage;
using Rights.Domain.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public IQueryable<Environment> GetFiltered(EnvironmentAccessRight accessRight)
        {
            return _dbContext.Set<Environment>()
                .FilteredByRights(accessRight);
        }
    }

    internal static class EnvironmentQueryableExtensions
    {
        public static IQueryable<Environment> FilteredByRights(this IQueryable<Environment> environments, EnvironmentAccessRight environmentAccessRight)
        {
            return environments
                .FilteredByStandardRights(environmentAccessRight.AccessRight)
                .FilteredByPurposes(environmentAccessRight.Purposes);
        }

        private static IQueryable<Environment> FilteredByStandardRights(this IQueryable<Environment> environments, AccessRight accessRight)
        {
            return accessRight switch
            {
                NoAccessRight _ => new List<Environment>().AsQueryable(),
                DistributorCodeAccessRight r => environments.Where(e => e.ActiveAccesses.Any(a => a.ConsumerId == r.DistributorCode)),
                AllAccessRight _ => environments,
                _ => throw new ApplicationException($"Unknown type of access right {accessRight}")
            };
        }

        private static IQueryable<Environment> FilteredByPurposes(this IQueryable<Environment> environments, PurposeAccessRight accessRight)
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
