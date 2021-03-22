using System.Threading.Tasks;

namespace Instances.Domain.Instances
{
    public interface ISubdomainValidator
    {
        Task ThrowIfInvalidAsync(string subdomain);
        Task<bool> IsAvailableAsync(string subdomain);
        Task<string> GetAvailableSubdomainAsync(string subdomain);
    }
}
