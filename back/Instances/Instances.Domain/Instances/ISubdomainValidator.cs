using System.Threading.Tasks;

namespace Instances.Domain.Instances
{
    public interface ISubdomainValidator
    {
        Task ThrowIfInvalidAsync(string subdomain);
        bool IsAvailable(string subdomain);
        string GetAvailableSubdomain(string subdomain);
    }
}
