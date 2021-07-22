using System.Threading.Tasks;

namespace Instances.Domain.Instances
{
    public interface ISubdomainValidator
    {
        Task ThrowIfInvalidAsync(string subdomain);
        Task ThrowIfUnavailableAsync(string subdomain);
        Task<bool> IsAvailableAsync(string subdomain);
        Task<string> GetAvailableSubdomainByPrefixAsync(string prefix);
    }
}
