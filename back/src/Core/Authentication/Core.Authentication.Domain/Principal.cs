using Lucca.Core.Shared.Domain.Contracts.Principals;
using Shared.Domain.Models;
using System;

namespace Core.Authentication.Domain
{
    public class Principal
    {
        public PrincipalType Type => PrincipalType.User;
        public Guid Token { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
