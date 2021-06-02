using Billing.Cmrr.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Cmrr.Application.Interfaces
{
    public interface IContractAnalyticSituationsService
    {
        Task<IEnumerable<IGrouping<AxisSection, ContractAnalyticSituation>>> GetOrderedSituationsAsync(CmrrAxis axis, IEnumerable<CmrrContractSituation> contractSituation);
    }
}
