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

        public async Task<IEnumerable<IGrouping<AxisSection, ContractAxisSectionSituation>>> GetOrderedSituationsAsync(CmrrAxis axis, IEnumerable<CmrrContractSituation> contractSituation)
        {
            var breakdowns = await GetBreakdownsPerProductIdAsync(axis);

            return contractSituation
                .SelectMany(s => ToAnalyticSituations(s, breakdowns[s.Contract.ProductId]))
                .OrderByDescending(s => s.PartialDiff)
                .GroupBy(analyticSituation => analyticSituation.Breakdown.AxisSection);
        }

        private IEnumerable<ContractAxisSectionSituation> ToAnalyticSituations(CmrrContractSituation contractSituation, List<Breakdown> breakdowns)
        {
            return breakdowns.Select(b => new ContractAxisSectionSituation(b, contractSituation));
        }

        private async Task<Dictionary<int, List<Breakdown>>> GetBreakdownsPerProductIdAsync(CmrrAxis axis)
        {
            var products = await _productsStore.GetNonFreeProductsAsync();

            return products.ToDictionary(p => p.Id, GetBreakdownsForProduct(axis));
        }

        private Func<Product, List<Breakdown>> GetBreakdownsForProduct(CmrrAxis axis)
        {
            return axis switch
            {
                CmrrAxis.Product => p => new List<Breakdown>(1) { new Breakdown { AxisSection = new AxisSection { Id = p.FamilyId, Name = p.Family.Name }, Ratio = 1 } },
                CmrrAxis.Solution => p => GetSolutionBreakdowns(p),
                CmrrAxis.BusinessUnit => p => GetBusinessUnitBreakdowns(p),
                _ => throw new BadRequestException($"Axis {axis} has no corresponding breakdowns")
            };
        }

        private List<Breakdown> GetBusinessUnitBreakdowns(Product p)
        {
            var shares = p.ProductSolutions.Select(ps => ps.Solution).Select(s => s.BusinessUnit)
                .Select(bu => new BreakdownShare(new AxisSection { Id = bu.Id, Name = bu.Name }, 1))
                .ToList();

            return ToBreakdowns(shares).ToList();
        }

        private List<Breakdown> GetSolutionBreakdowns(Product p)
        {
            var shares = p.ProductSolutions
                .Select(ps => new BreakdownShare(new AxisSection { Id = ps.SolutionId, Name = ps.Solution.Name }, 1))
                .ToList();

            return ToBreakdowns(shares).ToList();
        }

        private IEnumerable<Breakdown> ToBreakdowns(List<BreakdownShare> sharePerIds)
        {
            var sum = sharePerIds.Sum(s => s.Share);
            var ratioPartialSum = 0m;

            for (var i = 0; i < sharePerIds.Count - 1; i++)
            {
                var breakdownShare = sharePerIds[i];
                var ratio = breakdownShare.Share / sum;
                ratioPartialSum += ratio;
                yield return new Breakdown { AxisSection = breakdownShare.AxisSection, Ratio = ratio };
            }

            var lastBreakdownShare = sharePerIds.Last();

            yield return new Breakdown { AxisSection = lastBreakdownShare.AxisSection, Ratio = 1 - ratioPartialSum };

        }

        private class BreakdownShare
        {
            public AxisSection AxisSection { get; set; }

            public int Share { get; set; }

            public BreakdownShare(AxisSection axisSection, int share)
            {
                AxisSection = axisSection;
                Share = share;
            }
        }
    }
}
