using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Situation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Cmrr.Application.Interfaces
{
    public interface IContractAxisSectionSituationsService
    {
        Task<IEnumerable<ContractAxisSectionSituation>> GetAxisSectionSituationsAsync(CmrrAxis axis, IEnumerable<CmrrContractSituation> contractSituation);
    }
}
