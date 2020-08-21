using Lucca.Core.Rights.Abstractions.Principals;
using Lucca.Core.Shared.Domain.Contracts.Principals;
using System;

namespace Core.Authentication.Domain
{
    public class ApiKey : IApiKey
    {
        public int Id { get; set; }
        public PrincipalType Type => PrincipalType.ApiKey;
        public Guid Token { get; set; }
        public string Name { get; set; }
    }
}
