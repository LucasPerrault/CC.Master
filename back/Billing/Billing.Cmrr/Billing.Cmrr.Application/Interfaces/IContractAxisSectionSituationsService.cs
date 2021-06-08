using Billing.Cmrr.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Cmrr.Application.Interfaces
{
    public interface IContractAxisSectionSituationsService
    {
        Task<IEnumerable<ContractAxisSectionSituation>> GetAxisSectionSituationsAsync(CmrrAxis axis, IEnumerable<CmrrContractSituation> contractSituation);
    }
}
