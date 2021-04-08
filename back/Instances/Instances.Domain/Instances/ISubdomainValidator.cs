using System.Threading.Tasks;

namespace Instances.Domain.Instances
{
    public interface ISubdomainValidator
    {
        Task<string> GetSubdomainAsync(string candidate, bool useAsPrefix);
        Task ThrowIfInvalidAsync(string subdomain);
        bool IsAvailable(string subdomain);
        string GetAvailableSubdomainByPrefix(string prefix);
    }
}
