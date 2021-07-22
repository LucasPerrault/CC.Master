using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Microsoft.EntityFrameworkCore;
using Rights.Domain.Filtering;
using Storage.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Users.Domain;
using Users.Domain.Filtering;

namespace Users.Infra.Storage.Stores
{
    public class UsersStore : IUsersStore
    {
        private readonly UsersDbContext _context;
        private readonly IQueryPager _queryPager;

        public UsersStore(UsersDbContext context, IQueryPager queryPager)
        {
            _context = context;
            _queryPager = queryPager;
        }

        public async Task<Page<SimpleUser>> GetAsync(IPageToken pageToken, UsersFilter filter, AccessRight access)
        {
            var queryable = await GetQueryableAsync(filter, access);
            return await _queryPager.ToPageAsync(queryable, pageToken);
        }

        public async Task<List<SimpleUser>> GetAllAsync(UsersFilter filter, AccessRight accessRight)
        {
            var queryable = await GetQueryableAsync(filter, accessRight);
            return queryable.ToList();
        }

        public async Task<SimpleUser> GetByIdAsync(int id, AccessRight accessRight)
        {
            return await Users
                .Where(await ToExpressionAsync(accessRight))
                .SingleOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> ExistsByIdAsync(int userId, AccessRight accessRight)
        {
            return await Users
                .Where(await ToExpressionAsync(accessRight))
                .AnyAsync(u => u.Id == userId);
        }

        internal async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        internal Task CreateAsync(SimpleUser user)
        {
            _context.Add(user);
            return SaveChangesAsync();
        }

        private IQueryable<SimpleUser> Users => _context
            .Set<SimpleUser>();

        private async Task<IQueryable<SimpleUser>> GetQueryableAsync(UsersFilter filter, AccessRight accessRight)
        {
            return Users
                .WhereMatches(filter)
                .Where(await ToExpressionAsync(accessRight));
        }

        private async Task<Expression<Func<SimpleUser, bool>>> ToExpressionAsync(AccessRight accessRight)
        {
            return accessRight switch
            {
                NoAccessRight _ => _ => false,
                AllAccessRight _ => _ => true,
                DistributorAccessRight right => user => user.DistributorId == right.DistributorId,
                _ => throw new ApplicationException($"Unhandled access right {typeof(AccessRight)}")
            };
        }
    }

    internal static class UserQueryableExtensions
    {
        public static IQueryable<SimpleUser> WhereMatches(this IQueryable<SimpleUser> users, UsersFilter filter)
        {
            return users
                .Apply(filter.IsActive).To(u => u.IsActive)
                .WhenNotNullOrEmpty(filter.Search).ApplyWhere(u => u.FirstName.Contains(filter.Search) || u.LastName.Contains(filter.Search));
        }
    }
}
