using AdvancedFilters.Domain.DataSources;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AdvancedFilters.Infra.Services
{
    public class FetchAuthenticator
    {
        public Action<HttpRequestMessage> Authenticate(IDataSourceAuthentication dataSourceAuthentication)
        {
            return dataSourceAuthentication switch
            {
                AuthorizationAuthentication authorizationAuth => Authenticate(authorizationAuth),
                _ => throw new ApplicationException($"Authentication type {dataSourceAuthentication.GetType()} not supported")
            };
        }

        private Action<HttpRequestMessage> Authenticate(AuthorizationAuthentication authAuth)
        {
            return msg => msg.Headers.Authorization = new AuthenticationHeaderValue(authAuth.Scheme, authAuth.Parameter);
        }
    }
}
