using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Users.Domain;

namespace Users.Infra.Storage.Stores
{
    public class UsersStore
    {
        private readonly UsersDbContext _context;

        public UsersStore(UsersDbContext context)
        {
            _context = context;
        }

        public Task<SimpleUser> GetByIdAsync(int id)
        {
            return _context.Set<SimpleUser>().SingleOrDefaultAsync(u => u.Id == id);
        }

        public Task<bool> ExistsByIdAsync(int userId)
        {
            return _context.Set<SimpleUser>().AnyAsync(u => u.Id == userId);
        }

        public async Task<List<SimpleUser>> GetAllAsync()
        {
            return await _context.Set<SimpleUser>()
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task CreateAsync(User user)
        {
            var simpleUser = new SimpleUser
            {
                Id = user.Id,
                DepartmentId = user.DepartmentId,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            _context.Add(simpleUser);
            return SaveChangesAsync();
        }
    }
}
