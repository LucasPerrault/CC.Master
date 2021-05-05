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

        public async Task<SimpleUser> GetByIdAsync(int id, AccessRight right)
        {
            return await _context
                .Set<SimpleUser>()
                .Where(await ToExpressionAsync(right))
                .SingleOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> ExistsByIdAsync(int userId, AccessRight right)
        {
            return await _context
                .Set<SimpleUser>()
                .Where(await ToExpressionAsync(right))
                .AnyAsync(u => u.Id == userId);
        }

        public async Task<List<SimpleUser>> GetAllAsync(UsersFilter filter, AccessRight right)
        {
            return await _context
                .Set<SimpleUser>()
                .Where(ToExpression(filter))
                .Where(await ToExpressionAsync(right))
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

        public Expression<Func<SimpleUser, bool>> ToExpression(UsersFilter filter)
        {
            var expressions = new List<Expression<Func<SimpleUser, bool>>>();

            if (filter.IsActive != BoolCombination.Both)
            {
                expressions.Add(u => u.IsActive == filter.IsActive.ToBoolean());
            }

            if (!string.IsNullOrEmpty(filter.Search))
            {
                expressions.Add(u => u.FirstName.Contains(filter.Search) || u.LastName.Contains(filter.Search));
            }

            return expressions.ToArray().CombineSafely();
        }

        public async Task<Expression<Func<SimpleUser, bool>>> ToExpressionAsync(AccessRight accessRight)
        {
            return accessRight switch
            {
                NoAccessRight _ => _ => false,
                AllAccessRight _ => _ => true,
                DistributorCodeAccessRight right => await ToDistributorExpression(right.DistributorCode)
            };
        }

        private async Task<Expression<Func<SimpleUser, bool>>> ToDistributorExpression(string distributorCode)
        {
            var distributor = await _distributorsStore.GetByCodeAsync(distributorCode);
            return user => user.DepartmentId == distributor.DepartmentId;
        }
    }
}
