using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Cmrr.Application
{
    public class ContractAxisSectionSituationsService : IContractAxisSectionSituationsService
    {
        private readonly IBreakdownService _breakdownService;

        public ContractAxisSectionSituationsService(IBreakdownService breakdownService)
        {
            _breakdownService = breakdownService;
        }

        public async Task<IEnumerable<ContractAxisSectionSituation>> GetAxisSectionSituationsAsync(CmrrAxis axis, IEnumerable<CmrrContractSituation> contractSituation)
        {
            var breakdowns = await _breakdownService.GetBreakdownsAsync(axis);
            var breakdownsPerProductId = breakdowns
                .GroupBy(b => b.ProductId)
                .ToDictionary(g => g.Key, g => g.ToList());

            return contractSituation
                .SelectMany(s => ToAxisSectionSituations(s, breakdownsPerProductId[s.Contract.ProductId]));
        }


        private IEnumerable<ContractAxisSectionSituation> ToAxisSectionSituations(CmrrContractSituation contractSituation, List<Breakdown> breakdowns)
        {
            return breakdowns.Select(b => new ContractAxisSectionSituation(b, contractSituation));
        }
    }
}
