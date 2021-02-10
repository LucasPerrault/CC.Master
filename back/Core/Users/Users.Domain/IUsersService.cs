using System;
using System.Threading.Tasks;

namespace Users.Domain
{
    public interface IUsersService
    {
        Task<User> GetByTokenAsync(Guid token);
    }
}
