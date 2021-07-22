using Lucca.Core.Api.Abstractions.Paging;
using System.Security.Claims;
using System.Threading.Tasks;
using Users.Domain;
using Users.Domain.Filtering;

namespace Users.Application
{
    public class UsersRepository
    {
        private readonly ClaimsPrincipal _principal;
        private readonly IUsersStore _usersStore;
        private readonly UserRightsFilter _rightsFilter;

        public UsersRepository(ClaimsPrincipal principal, IUsersStore usersStore, UserRightsFilter rightsFilter)
        {
            _principal = principal;
            _usersStore = usersStore;
            _rightsFilter = rightsFilter;
        }
        public async Task<Page<SimpleUser>> GetAsync(IPageToken pageToken, UsersFilter filter)
        {
            return await _usersStore.GetAsync(pageToken, filter, await _rightsFilter.GetAccessAsync(_principal));
        }
    }
}
