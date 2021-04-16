using Authentication.Domain;
using System;
using System.Threading.Tasks;
using Users.Domain;

namespace Authentication.Infra.Services
{
    public interface IUserAuthenticationRemoteService
    {
        Task<Principal> GetUserPrincipalAsync(Guid token);
    }

    public class UserAuthenticationRemoteService : IUserAuthenticationRemoteService
    {
        private readonly IUsersService _usersService;
        private readonly AuthenticationCache _cache;

        public UserAuthenticationRemoteService(IUsersService usersService, AuthenticationCache cache)
        {
            _usersService = usersService;
            _cache = cache;
        }

        // will be called with token of current principal
        public async Task<Principal> GetUserPrincipalAsync(Guid token)
        {
            var user = await GetUserAsync(token);
            if (user == null)
            {
                return null;
            }

            return new Principal
            {
                Token = token,
                UserId = user.Id,
                User = user
            };
        }

        private async Task<User> GetUserAsync(Guid token)
        {
            if (_cache.TryGetUser(token, out var cachedUser))
            {
                return cachedUser;
            }

            var user = await _usersService.GetByTokenAsync(token);
            if (user == null)
            {
                return null;
            }

            _cache.Cache(token, user);
            return user;
        }
    }
}
