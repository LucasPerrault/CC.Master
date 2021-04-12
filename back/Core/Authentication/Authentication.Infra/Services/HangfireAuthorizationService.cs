using Authentication.Infra.Configurations;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Authentication.Infra.Services
{
    public class HangfireAuthorizationService
    {
        public const string SignatureHeader = "X-Lucca-Signature";

        private readonly HangfireAuthenticationConfiguration _configuration;

        public HangfireAuthorizationService(HangfireAuthenticationConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ThrowIfUnauthorized(HttpRequest request)
        {
            var hasSignature = request.Headers.TryGetValue(SignatureHeader, out var signatures);

            if (!hasSignature)
            {
                throw new ForbiddenException();
            }

            var headerSignature = signatures.First();
            var computedSignature = GetSignature(_configuration.SharedSecret, GetUrl(request));

            if (!computedSignature.Equals(headerSignature))
            {
                throw new ForbiddenException();
            }
        }

        private static string GetSignature(Guid sharedSecret, string url)
        {
            var secretBytes = Encoding.UTF8.GetBytes(sharedSecret.ToString());
            var data = Encoding.UTF8.GetBytes(url);

            using var hasher = new HMACSHA256(secretBytes);
            return Convert.ToBase64String(hasher.ComputeHash(data));
        }

        private static string GetUrl(HttpRequest request)
        {
            var builder = new UriBuilder(request.Host.Value)
            {
                Scheme = null!,
                Port = -1
            };

            return builder.ToString();
        }
    }
}
