using Billing.Cmrr.Domain.Situation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Cmrr.Application.Interfaces
{
    public interface ICmrrSituationsService
    {
        Task<CmrrSituation> GetSituationAsync(CmrrFilter filter);
        Task<IReadOnlyCollection<CmrrClient>> GetAcquiredClientsAsync(CmrrFilter filter);
        Task<IReadOnlyCollection<CmrrClient>> GetTerminatedClientsAsync(CmrrFilter filter);
    }
}
