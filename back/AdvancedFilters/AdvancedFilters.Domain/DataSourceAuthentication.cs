﻿using System;

namespace AdvancedFilters.Domain
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

        public DataSourceAuthType Type => DataSourceAuthType.Authorization;

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
        public DataSourceAuthType Type => DataSourceAuthType.Lucca;

        public LuccaAuthentication(Guid token) : base("Lucca", $"webservice={token}")
        { }
    }
}
