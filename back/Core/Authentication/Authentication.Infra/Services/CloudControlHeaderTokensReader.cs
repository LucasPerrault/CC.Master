﻿using Lucca.Core.Authentication.Abstractions.Methods;
using Lucca.Core.Authentication.Tokens;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Authentication.Infra.Services
{
    public class CloudControlHeaderTokensReader : SpecializedTokensReader
    {
        private const string AuthorizationKey = "Authorization";
        private const string Scheme = "CloudControl";

        public override TokenLocations Location => TokenLocations.Headers;

        protected override bool IsKeyPresent(HttpRequest httpRequest, string key)
        {
            if (!httpRequest.Headers.ContainsKey(AuthorizationKey))
            {
                return false;
            }

            var header = httpRequest.Headers[AuthorizationKey].FirstOrDefault()?.ToUpperInvariant();
            return header != null
                   && header.StartsWith(GetHeaderPrefix(key));
        }

        protected override string GetValue(HttpRequest httpRequest, string key)
        {
            var authorization = httpRequest.Headers[AuthorizationKey].First().ToUpperInvariant();
            return authorization.Replace(GetHeaderPrefix(key), "");
        }

        public static bool TryGetGuidValue(HttpRequest httpRequest, string key, out Guid guid)
        {
            var authorization = httpRequest.Headers[AuthorizationKey].FirstOrDefault()?.ToUpperInvariant();
            if (
                string.IsNullOrEmpty(authorization)
                || !Guid.TryParse(authorization.Replace(GetHeaderPrefix(key), ""), out var parsed)
               )
            {
                guid = default;
                return false;
            }

            guid = parsed;
            return true;
        }

        private static string GetHeaderPrefix(string key) => $"{Scheme} {key}=".ToUpperInvariant();
    }
}
