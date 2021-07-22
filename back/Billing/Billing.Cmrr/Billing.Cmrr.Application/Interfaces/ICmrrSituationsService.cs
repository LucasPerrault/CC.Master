using Billing.Cmrr.Domain.Situation;
using System.Threading.Tasks;

namespace Billing.Cmrr.Application.Interfaces
{
    public interface ICmrrSituationsService
    {
        Task<CmrrSituation> GetSituationAsync(CmrrFilter filter);
    }
}
