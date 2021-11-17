using Lucca.Core.Api.Abstractions.Paging;
using Rights.Domain.Filtering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Contracts.Interfaces
{
    public interface IContractsStore
    {
        Task<Contract> GetSingleAsync(AccessRight accessRight, ContractFilter contractFilter);
        Task<Page<Contract>> GetPageAsync(AccessRight accessRight, ContractFilter filter, IPageToken pageToken);
        Task<List<Contract>> GetAsync(AccessRight accessRight, ContractFilter filter);
        IQueryable<Contract> GetQueryable(ContractFilter filter);
        Task<ContractComment> GetCommentAsync(AccessRight accessRight, int contractId);
    }
}
