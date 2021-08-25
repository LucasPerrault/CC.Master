using System;

namespace AdvancedFilters.Domain.DataSources
{
    public interface IDataSourceAuthentication
    { }

    public class AuthorizationAuthentication : IDataSourceAuthentication
    {
        public string Scheme { get; }
        public string Parameter { get; }

        public AuthorizationAuthentication(string scheme, string parameter)
        {
            Scheme = scheme;
            Parameter = parameter;
        }

        public void Apply(IDataSourceAuthenticator authenticator)
        {
            authenticator.Authenticate(this);
        }
    }

    public class LuccaAuthentication : AuthorizationAuthentication
    {
        public LuccaAuthentication(Guid token)
            : base("Lucca", $"webservice={token}")
        { }
    }
}
