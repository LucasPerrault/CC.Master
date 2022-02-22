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
            var queryable = GetQueryable(filter, access);
            return await _queryPager.ToPageAsync(queryable, pageToken);
        }

        public async Task<List<SimpleUser>> GetAllAsync(UsersFilter filter, AccessRight accessRight)
        {
            return await GetQueryable(filter, accessRight).ToListAsync();
        }

        public async Task<SimpleUser> GetByIdAsync(int id, AccessRight accessRight)
        {
            return await GetQueryable(accessRight).SingleOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> ExistsByIdAsync(int userId, AccessRight accessRight)
        {
            return await GetQueryable(accessRight).AnyAsync(u => u.Id == userId);
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

        private IQueryable<SimpleUser> GetQueryable(UsersFilter filter, AccessRight accessRight)
        {
            return Users
                .WithAccess(accessRight)
                .WhereMatches(filter);
        }

        private IQueryable<SimpleUser> GetQueryable(AccessRight accessRight)
        {
            return Users.WithAccess(accessRight);
        }
    }

    internal static class UserQueryableExtensions
    {
        public static IQueryable<SimpleUser> WhereMatches(this IQueryable<SimpleUser> users, UsersFilter filter)
        {
            return users
                .Apply(filter.IsActive).To(u => u.IsActive)
                .WhenNotNullOrEmpty(filter.Search).ApplyWhere(u => filter.Search.Contains(u.FirstName) || filter.Search.Contains(u.LastName));
        }

        public static IQueryable<SimpleUser> WithAccess(this IQueryable<SimpleUser> users, AccessRight accessRight)
        {
            return users.Where(accessRight.ToExpression());
        }

        private static Expression<Func<SimpleUser, bool>> ToExpression(this AccessRight accessRight)
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
}
