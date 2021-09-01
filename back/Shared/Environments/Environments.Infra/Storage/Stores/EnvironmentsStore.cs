using Environments.Domain.Storage;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
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
        private readonly IQueryPager _queryPager;

        public EnvironmentsStore(EnvironmentsDbContext dbContext, IQueryPager queryPager)
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
        }

        public Task<List<Environment>> GetAsync(List<EnvironmentAccessRight> rights, EnvironmentFilter filter)
        {
            return GetQueryable(rights, filter).ToListAsync();
        }

        public Task<Page<Environment>> GetAsync(IPageToken page, List<EnvironmentAccessRight> rights, EnvironmentFilter filter)
        {
            return _queryPager.ToPageAsync(GetQueryable(rights, filter), page);
        }

        private IQueryable<Environment> GetQueryable(List<EnvironmentAccessRight> rights, EnvironmentFilter filter)
        {
            return _dbContext.Set<Environment>()
                .Include(e => e.ActiveAccesses).ThenInclude(a => a.Consumer)
                .Include(e => e.ActiveAccesses).ThenInclude(a => a.Access)
                .ForRights(rights)
                .FilterBy(filter);
        }
    }

    internal static class EnvironmentQueryableExtensions
    {
        public static IQueryable<Environment> FilterBy(this IQueryable<Environment> environments, EnvironmentFilter filter)
        {
            return environments
                .Apply(filter.Subdomain).To(e => e.Subdomain)
                .Apply(filter.IsActive).To(e => e.IsActive)
                .WhenNotNullOrEmpty(filter.Ids).ApplyWhere(e => filter.Ids.Contains(e.Id))
                .WhenNotNullOrEmpty(filter.Search).ApplyWhere(e => e.Subdomain.Contains(filter.Search))
                .WhenNotNullOrEmpty(filter.Purposes).ApplyWhere(e => filter.Purposes.Contains(e.Purpose))
                .WhenNotNull(filter.Domains).ApplyWhere(e => filter.Domains.Contains(e.Domain));
            // The case with an empty filter.Domains comes from the api user asking for non-existing domains,
            // whereas the case with a null filter.Domains comes from the api user not asking for any domain in particular
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

        private static Expression<Func<Environment, bool>> WithPurposes(PurposeAccessRight accessRight)
        {
            return accessRight switch
            {
                AllPurposeAccessRight _ => e => true,
                SomePurposesAccessRight r => e => r.Purposes.Contains(e.Purpose),
                _ => throw new ApplicationException($"Unknown type of purpose access right {accessRight}")
            };
        }

        private static Expression<Func<Environment, bool>> WithAccessRight(AccessRight accessRight)
        {
            return accessRight switch
            {
                NoAccessRight _ => e => false,
                DistributorAccessRight r => e => e.ActiveAccesses.Any(a => a.ConsumerId == r.DistributorId),
                AllAccessRight _ => e => true,
                _ => throw new ApplicationException($"Unknown type of purpose access right {accessRight}")
            };
        }
    }
}
