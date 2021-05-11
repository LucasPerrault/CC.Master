using Distributors.Domain;
using Microsoft.EntityFrameworkCore;
using Rights.Domain.Filtering;
using Storage.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tools;
using Users.Domain;
using Users.Domain.Filtering;

namespace Users.Infra.Storage.Stores
{
    public class UsersStore : IUsersStore
    {
        private readonly UsersDbContext _context;
        private readonly IDistributorsStore _distributorsStore;

        public UsersStore(UsersDbContext context, IDistributorsStore distributorsStore)
        {
            _context = context;
            _distributorsStore = distributorsStore;
        }

        public async Task<SimpleUser> GetByIdAsync(int id, AccessRight accessRight)
        {
            return await _context
                .Set<SimpleUser>()
                .Where(await ToExpressionAsync(accessRight))
                .SingleOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> ExistsByIdAsync(int userId, AccessRight accessRight)
        {
            return await _context
                .Set<SimpleUser>()
                .Where(await ToExpressionAsync(accessRight))
                .AnyAsync(u => u.Id == userId);
        }

        public async Task<List<SimpleUser>> GetAllAsync(UsersFilter filter, AccessRight accessRight)
        {
            return await _context
                .Set<SimpleUser>()
                .WhereMatches(filter)
                .Where(await ToExpressionAsync(accessRight))
                .ToListAsync();
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

        public async Task<Expression<Func<SimpleUser, bool>>> ToExpressionAsync(AccessRight accessRight)
        {
            return accessRight switch
            {
                NoAccessRight _ => _ => false,
                AllAccessRight _ => _ => true,
                DistributorCodeAccessRight right => await ToDistributorExpressionAsync(right.DistributorCode),
                _ => throw new ApplicationException($"Unhandled access right {typeof(AccessRight)}")
            };
        }

        private async Task<Expression<Func<SimpleUser, bool>>> ToDistributorExpressionAsync(string distributorCode)
        {
            var distributor = await _distributorsStore.GetByCodeAsync(distributorCode);
            return user => user.DepartmentId == distributor.DepartmentId;
        }
    }

    internal static class UserQueryableExtensions
    {
        public static IQueryable<SimpleUser> WhereMatches(this IQueryable<SimpleUser> users, UsersFilter filter)
        {
            return users
                .WhenNotBoth(filter.IsActive).ApplyWhere(u => u.IsActive == filter.IsActive.ToBoolean())
                .WhenNotNullOrEmpty(filter.Search).ApplyWhere(u => u.FirstName.Contains(filter.Search) || u.LastName.Contains(filter.Search));
        }
    }
}
