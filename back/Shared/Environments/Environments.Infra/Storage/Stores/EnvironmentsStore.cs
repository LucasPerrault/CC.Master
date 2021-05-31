using Environments.Domain.Storage;
using Lucca.Core.Shared.Domain.Expressions;
using Microsoft.EntityFrameworkCore;
using Rights.Domain.Filtering;
using Storage.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public Task<List<Environment>> GetAsync(List<EnvironmentAccessRight> rights, EnvironmentFilter filter)
        {
            return _dbContext.Set<Environment>()
                .ForRights(rights)
                .FilterBy(filter)
                .ToListAsync();
        }
    }

    internal static class EnvironmentQueryableExtensions
    {
        public static IQueryable<Environment> FilterBy(this IQueryable<Environment> environments, EnvironmentFilter filter)
        {
            return environments
                .Apply(filter.Subdomain).To(e => e.Subdomain)
                .Apply(filter.IsActive).To(e => e.IsActive);
        }

        public static IQueryable<Environment> ForRights
        (
            this IQueryable<Environment> environments,
            List<EnvironmentAccessRight> accessRights
        )
        {
            if (!accessRights.Any())
            {
                return new List<Environment>().AsQueryable();
            }

            var expressions = accessRights
                .Select(r => WithPurposes(r.Purposes).SmartAndAlso(WithAccessRight(r.AccessRight)))
                .ToArray();

            return environments.Where(expressions.CombineSafelyOr());
        }

        public static Expression<Func<Environment, bool>> WithPurposes(PurposeAccessRight accessRight)
        {
            return accessRight switch
            {
                AllPurposeAccessRight _ => e => true,
                SomePurposesAccessRight r => e => r.Purposes.Contains(e.Purpose),
                _ => throw new ApplicationException($"Unknown type of purpose access right {accessRight}")
            };
        }

        public static Expression<Func<Environment, bool>> WithAccessRight(AccessRight accessRight)
        {
            return accessRight switch
            {
                NoAccessRight _ => e => false,
                DistributorCodeAccessRight r => e => e.ActiveAccesses.Any(a => a.ConsumerId == r.DistributorCode),
                _ => throw new ApplicationException($"Unknown type of purpose access right {accessRight}")
            };
        }
    }
}
