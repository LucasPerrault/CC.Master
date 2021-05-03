using Lucca.Core.Shared.Domain.Exceptions;
using System.Threading.Tasks;

namespace Instances.Domain.Instances
{
    public interface ISubdomainGenerator
    {
        Task<string> GetSubdomainAsync(string candidate, bool useAsPrefix);
    }

    public class SubdomainGenerator : ISubdomainGenerator
    {
        private readonly ISubdomainValidator _validator;

        public SubdomainGenerator(ISubdomainValidator validator)
        {
            _validator = validator;
        }

        public async Task<string> GetSubdomainAsync(string candidate, bool useAsPrefix)
        {
            await _validator.ThrowIfInvalidAsync(candidate);

            if (await _validator.IsAvailableAsync(candidate))
            {
                return candidate;
            }

            if (!useAsPrefix)
            {
                throw new BadRequestException($"Subdomain {candidate} is not available");
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