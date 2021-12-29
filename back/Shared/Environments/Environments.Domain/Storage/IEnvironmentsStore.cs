using Lucca.Core.Api.Abstractions.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Environments.Domain.Storage
{
    public interface IEnvironmentsStore
    {
        Task<List<Environment>> GetAsync(List<EnvironmentAccessRight> rights, EnvironmentFilter filter);
        Task<Page<Environment>> GetAsync(IPageToken page, List<EnvironmentAccessRight> rights, EnvironmentFilter filter);
        Task UpdateSubDomainAsync(Environment environement, string newName);
    }
}
