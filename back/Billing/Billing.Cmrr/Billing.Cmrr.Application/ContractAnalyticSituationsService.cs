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
    public class ContractAnalyticSituationsService : IContractAnalyticSituationsService
    {
        private readonly IProductsStore _productsStore;

        public ContractAnalyticSituationsService(IProductsStore productsStore)
        {
            _productsStore = productsStore;
        }

        public async Task<IEnumerable<IGrouping<AxisSection, ContractAnalyticSituation>>> GetOrderedSituationsAsync(CmrrAxis axis, IEnumerable<CmrrContractSituation> contractSituation)
        {
            var breakdowns = await GetBreakdownsPerProductIdAsync(axis);

            return contractSituation
                .SelectMany(s => ToAnalyticSituations(s, breakdowns[s.Contract.ProductId]))
                .OrderByDescending(s=>s.PartialDiff)
                .GroupBy(analyticSituation => analyticSituation.Breakdown.AxisSection);
        }

        private IEnumerable<ContractAnalyticSituation> ToAnalyticSituations(CmrrContractSituation contractSituation, List<Breakdown> breakdowns)
        {
            return breakdowns.Select(b => new ContractAnalyticSituation(b, contractSituation));
        }

        private async Task<Dictionary<int, List<Breakdown>>> GetBreakdownsPerProductIdAsync(CmrrAxis axis)
        {
            var products = await _productsStore.GetProductsAsync();

            return products.ToDictionary(p => p.Id, GetBreakdownsForProduct(axis));
        }

        private Func<Product, List<Breakdown>> GetBreakdownsForProduct(CmrrAxis axis)
        {
            return axis switch
            {
                CmrrAxis.Product => p => new List<Breakdown>(1) { new Breakdown { AxisSection = new AxisSection { Id = p.FamilyId, Name = p.Family.Name } , Ratio = 1 } },
                _ => throw new BadRequestException($"Axis {axis} has no corresponding breakdowns")
            };
        }
    }
}
