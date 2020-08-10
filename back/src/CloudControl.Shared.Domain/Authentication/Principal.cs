using CloudControl.Shared.Domain.Models;
using Lucca.Core.Shared.Domain.Contracts.Principals;
using System;

namespace CloudControl.Shared.Domain.Authentication
{
    public class Principal
    {
        public PrincipalType Type => PrincipalType.User;
        public Guid Token { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
