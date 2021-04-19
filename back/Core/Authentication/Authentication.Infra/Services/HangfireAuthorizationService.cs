using Authentication.Infra.Configurations;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Authentication.Infra.Services
{
    public class HangfireAuthorizationService
    {
        public const string SignatureHeader = "X-Lucca-Signature";

        private readonly HangfireAuthenticationConfiguration _configuration;
        private readonly SignatureAuthenticationService _service;

        public HangfireAuthorizationService
        (
            HangfireAuthenticationConfiguration configuration,
            SignatureAuthenticationService service
        )
        {
            _configuration = configuration;
            _service = service;
        }

        public void ThrowIfUnauthorized(HttpRequest request)
        {
            var hasSignature = request.Headers.TryGetValue(SignatureHeader, out var signatures);

            if (!hasSignature)
            {
                throw new ForbiddenException();
            }

            var headerSignature = signatures.First();
            var computedSignature = _service.GetSignature(_configuration.SharedSecret, request);

            if (!computedSignature.Equals(headerSignature))
            {
                throw new ForbiddenException();
            }
        }
    }
}
