using System.Threading.Tasks;

namespace Environments.Domain.Storage
{
    public interface IEnvironmentLogsStore
    {
        Task<EnvironmentLog> CreateAsync(EnvironmentLog log);
    }
}
