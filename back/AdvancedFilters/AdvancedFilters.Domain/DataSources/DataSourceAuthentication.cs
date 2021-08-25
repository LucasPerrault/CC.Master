using System;

namespace AdvancedFilters.Domain.DataSources
{
    public enum DataSourceAuthType
    {
        Authorization,
        Lucca
    }

    public interface IDataSourceAuthentication
    {
        public DataSourceAuthType Type { get; }
    }

    public class AuthorizationAuthentication : IDataSourceAuthentication
    {
        public virtual DataSourceAuthType Type => DataSourceAuthType.Authorization;

        public string Scheme { get; }
        public string Parameter { get; }

        public AuthorizationAuthentication(string scheme, string parameter)
        {
            Scheme = scheme;
            Parameter = parameter;
        }
    }

    public class LuccaAuthentication : AuthorizationAuthentication
    {
        public override DataSourceAuthType Type => DataSourceAuthType.Lucca;

        public LuccaAuthentication(Guid token)
            : base("Lucca", $"webservice={token}")
        { }
    }
}
