using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Users.Domain;

namespace Users.Infra.Storage.Stores
{
    public class UsersStore : IUsersStore
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

        public Task CreateAsync(SimpleUser user)
        {
            _context.Add(user);
            return SaveChangesAsync();
        }
    }
}
