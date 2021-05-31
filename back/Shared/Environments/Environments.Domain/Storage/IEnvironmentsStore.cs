using System.Collections.Generic;
using System.Threading.Tasks;

namespace Environments.Domain.Storage
{
    public interface IEnvironmentsStore
    {
        Task<List<Environment>> GetFilteredAsync(EnvironmentAccessRight accessRight, EnvironmentFilter filter);
    }
}
