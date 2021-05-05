using System.Collections.Generic;
using System.Threading.Tasks;

namespace Users.Domain
{
    public interface IUsersStore
    {
        Task<SimpleUser> GetByIdAsync(int id);
        Task<bool> ExistsByIdAsync(int userId);
        Task<List<SimpleUser>> GetAllAsync();
    }
}
