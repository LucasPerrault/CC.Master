using Authentication.Domain;
using System;
using System.Threading.Tasks;
using Users.Domain;

namespace Authentication.Infra.Services
{
    public class UserAuthenticationRemoteService
    {
        private readonly IUsersService _usersService;

        public UserAuthenticationRemoteService(IUsersService usersService)
        {
            _usersService = usersService;
        }

        // will be called with token of current principal
        public async Task<Principal> GetUserPrincipalAsync(Guid token)
        {
            try
            {
                var user = await _usersService.GetByTokenAsync(token);
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
            catch
            {
                return null;
            }

        }
    }
}
