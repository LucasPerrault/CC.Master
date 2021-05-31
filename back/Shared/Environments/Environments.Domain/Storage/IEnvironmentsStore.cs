using System.Collections.Generic;
using System.Threading.Tasks;

namespace Environments.Domain.Storage
{
    public interface IEnvironmentsStore
    {
        Task<List<Environment>> GetAsync(List<EnvironmentAccessRight> rights, EnvironmentFilter filter);
    }
}
