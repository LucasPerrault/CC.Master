using CloudControl.Shared.Domain.Authentication;
using CloudControl.Shared.Infra.Authentication.Configurations;
using CloudControl.Shared.Infra.Authentication.DTOs;
using CloudControl.Shared.Infra.Configuration;
using CloudControl.Shared.Infra.Remote.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudControl.Shared.Infra.Authentication
{
    public class AuthenticationRemoteService : HostRemoteService<PartenairesAuthServiceConfiguration>
    {
        private readonly AuthenticationConfiguration _authConfig;
        private readonly ApiKeysConfiguration _apiKeysConfig;
        private const string _authScheme = "Lucca";
        private const string _authType = "user";
        protected override string RemoteAppName => "Partenaires";

        public AuthenticationRemoteService(HttpClient httpClient, JsonSerializer jsonSerializer, AuthenticationConfiguration authConfig, ApiKeysConfiguration apiKeysConfig)
            : base(httpClient, jsonSerializer)
        {
            _authConfig = authConfig;
            _apiKeysConfig = apiKeysConfig;
        }
    }
}
