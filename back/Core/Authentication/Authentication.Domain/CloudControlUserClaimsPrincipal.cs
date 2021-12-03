using Lucca.Core.Authentication;
using Lucca.Core.Rights.Abstractions.Principals;
using Lucca.Core.Shared.Domain.Contracts.Principals;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Users.Domain;

namespace Authentication.Domain
{
    public class CloudControlUserClaimsPrincipal : ClaimsPrincipal
    {
        private readonly Principal _principal;

        public CloudControlUserClaimsPrincipal(Principal principal)
            : base(new ClaimsIdentity(
                new List<Claim>
                {
                    new Claim(ClaimTypes.Name, principal.User.Name),
                    new Claim(nameof(Principal.Type), principal.Type.ToString()),
                    new Claim(nameof(Principal.Token), principal.Token.ToString()),
                    new Claim(nameof(Principal.UserId), principal.UserId.ToString()),
                    new Claim(nameof(IUser.Id), principal.UserId.ToString()),
                    new Claim(nameof(IUser.DepartmentId), principal.User.DepartmentId.ToString()),
                    new Claim(nameof(IUser.EstablishmentId), principal.User.EstablishmentId.ToString()),
                    new Claim(nameof(IUser.ManagerId), principal.User.ManagerId.ToString())
                }, AuthenticationExtensions.LuccaScheme)) // TODO CcScheme ?
        {
            _principal = principal;
        }

        public PrincipalType Type => _principal.Type;
        public string Name => _principal.User.Name;
        public Guid Token => _principal.Token;
        public int? UserId => _principal.UserId;
        public User User => _principal.User;
        public bool IsLuccaAdmin => UserId == 0;
    }
}
