using Microsoft.AspNetCore.Http;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Authentication.Infra.Services
{
    public class SignatureAuthenticationService
    {
        public string GetSignature(Guid sharedSecret, HttpRequest request)
        {
            if (sharedSecret == null)
            {
                throw new ArgumentNullException(nameof(sharedSecret));
            }

            var url = GetUrl(request);
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
                Port = -1,
                Path = request.Path
            };

            return builder.ToString();
        }
    }
}
