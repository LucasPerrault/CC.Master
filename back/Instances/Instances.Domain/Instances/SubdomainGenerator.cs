using Lucca.Core.Shared.Domain.Exceptions;
using System.Threading.Tasks;

namespace Instances.Domain.Instances
{
    public interface ISubdomainGenerator
    {
        Task ThrowIfNotUsable(string candidate);
        Task<string> GetSubdomainFromPrefixAsync(string candidate);
    }

    public class SubdomainGenerator : ISubdomainGenerator
    {
        private readonly ISubdomainValidator _validator;

        public SubdomainGenerator(ISubdomainValidator validator)
        {
            _validator = validator;
        }

        public async Task ThrowIfNotUsable(string candidate)
        {
            await _validator.ThrowIfInvalidAsync(candidate);

            if (!await _validator.IsAvailableAsync(candidate))
            {
                throw new BadRequestException($"Subdomain {candidate} is not available");
            }
        }

        public async Task<string> GetSubdomainFromPrefixAsync(string candidate)
        {
            await _validator.ThrowIfInvalidAsync(candidate);

            if (await _validator.IsAvailableAsync(candidate))
            {
                return candidate;
            }

            var availableSubdomain = await _validator.GetAvailableSubdomainByPrefixAsync(candidate);
            if (string.IsNullOrEmpty(availableSubdomain))
            {
                throw new BadRequestException($"Subdomain {candidate} is not available (limit reached for this prefix)");
            }

            return availableSubdomain;
        }
    }
}
