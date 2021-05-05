using System.Collections.Generic;
using System.Threading.Tasks;
using Users.Domain;

namespace Users.Application
{
    public class UsersRepository
    {
        private readonly IUsersStore _usersStore;

        public UsersRepository(IUsersStore usersStore)
        {
            _usersStore = usersStore;
        }
        public Task<List<SimpleUser>> GetAsync()
        {
            return _usersStore.GetAllAsync();
        }
    }
}
