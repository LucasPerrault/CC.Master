using Lucca.Core.Api.Abstractions.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Environments.Domain.Storage
{
    public interface IEnvironmentsStore
    {
        Task<Environment> GetActiveByIdAsync(List<EnvironmentAccessRight> rights, int id);
        Task<List<Environment>> GetAsync(List<EnvironmentAccessRight> rights, EnvironmentFilter filter);
        Task<Page<Environment>> GetAsync(IPageToken page, List<EnvironmentAccessRight> rights, EnvironmentFilter filter);
        Task UpdateSubDomainAsync(Environment environement, string newName);
        Task<bool> HasAccessAsync(List<EnvironmentAccessRight> rights, int environmentId);
    }
}
