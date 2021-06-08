using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using Billing.Products.Domain;
using Billing.Products.Domain.Interfaces;
using Lucca.Core.Shared.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Cmrr.Application
{
    public class ContractAxisSectionSituationsService : IContractAxisSectionSituationsService
    {
        private readonly IProductsStore _productsStore;
        private readonly BreakdownService _breakdownService;

        public ContractAxisSectionSituationsService(IProductsStore productsStore, BreakdownService breakdownService)
        {
            _productsStore = productsStore;
            _breakdownService = breakdownService;
        }

        public async Task<IEnumerable<ContractAxisSectionSituation>> GetAxisSectionSituationsAsync(CmrrAxis axis, IEnumerable<CmrrContractSituation> contractSituation)
        {
            var breakdowns = await GetBreakdownsPerProductIdAsync(axis);

            return contractSituation
                .SelectMany(s => ToAxisSectionSituations(s, breakdowns[s.Contract.ProductId]));
        }

        private IEnumerable<ContractAxisSectionSituation> ToAxisSectionSituations(CmrrContractSituation contractSituation, List<Breakdown> breakdowns)
        {
            return breakdowns.Select(b => new ContractAxisSectionSituation(b, contractSituation));
        }

        private async Task<Dictionary<int, List<Breakdown>>> GetBreakdownsPerProductIdAsync(CmrrAxis axis)
        {
            var products = await _productsStore.GetNonFreeProductsAsync();

            return products.ToDictionary(p => p.Id, _breakdownService.GetBreakdownsForProduct(axis));
        }
    }
}
