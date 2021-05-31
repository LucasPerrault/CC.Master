using Rights.Domain.Filtering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Environments.Domain.Storage
{
    public interface IEnvironmentsStore
    {
        Task<List<Environment>> GetFilteredAsync(AccessRight accessRight, PurposeAccessRight purposeAccessRight, EnvironmentFilter filter);
    }
}
