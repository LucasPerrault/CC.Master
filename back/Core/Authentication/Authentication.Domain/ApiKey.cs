using Distributors.Domain.Models;
using Lucca.Core.Rights.Abstractions.Principals;
using Lucca.Core.Shared.Domain.Contracts.Principals;
using System;

namespace Authentication.Domain
{
    public class ApiKey : IApiKey
    {
        public int Id => 0;  // api key ids cannot be known by design ; we'll always assume "lucca user"
        public int DistributorId => Distributor.DefaultDistributorId;// api keys can't be linked to a distributor, we'll always assume Lucca
        public PrincipalType Type => PrincipalType.ApiKey;
        public Guid Token { get; set; }
        public string StorableId => Token.ToString().Substring(0, 18);
        public string Name { get; set; }
    }
}
