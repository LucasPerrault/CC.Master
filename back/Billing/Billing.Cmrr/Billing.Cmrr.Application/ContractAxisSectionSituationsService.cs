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

        public ContractAxisSectionSituationsService(IProductsStore productsStore)
        {
            _productsStore = productsStore;
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

            return products.ToDictionary(p => p.Id, GetBreakdownsForProduct(axis));
        }

        private Func<Product, List<Breakdown>> GetBreakdownsForProduct(CmrrAxis axis)
        {
            return axis switch
            {
                CmrrAxis.Product => p => new List<Breakdown>(1) { new Breakdown { AxisSection = new AxisSection { Id = p.FamilyId, Name = p.Family.Name }, Ratio = 1, SubSection = p.Name } },
                CmrrAxis.BusinessUnit => GetBusinessUnitBreakdowns,
                _ => throw new BadRequestException($"Axis {axis} has no corresponding breakdowns")
            };
        }

        private List<Breakdown> GetBusinessUnitBreakdowns(Product p)
        {
            var shares = p.ProductSolutions.Select(ps => ps.Solution)
                .Select(s => new BreakdownShare(new AxisSection { Id = s.BusinessUnitId, Name = s.BusinessUnit.Name }, s.Name, 1))
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
                yield return new Breakdown { AxisSection = breakdownShare.AxisSection, Ratio = ratio, SubSection = breakdownShare.SubSection};
            }

            var lastBreakdownShare = sharePerIds.Last();

            yield return new Breakdown { AxisSection = lastBreakdownShare.AxisSection, Ratio = 1 - ratioPartialSum };

        }

        private class BreakdownShare
        {
            public AxisSection AxisSection { get; }

            public int Share { get; }

            public string SubSection { get; }

            public BreakdownShare(AxisSection axisSection, string subSection, int share)
            {
                AxisSection = axisSection;
                Share = share;
                SubSection = subSection;
            }
        }
    }
}
