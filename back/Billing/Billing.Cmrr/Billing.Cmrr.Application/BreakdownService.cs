using Billing.Cmrr.Domain;
using Billing.Products.Domain;
using Billing.Products.Domain.Interfaces;
using Lucca.Core.Shared.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Billing.Cmrr.Application
{
    public class BreakdownService
    {
        private readonly IProductsStore _productsStore;

        public BreakdownService(IProductsStore productsStore)
        {
            _productsStore = productsStore;
        }

        public Func<Product, List<Breakdown>> GetBreakdownsForProduct(CmrrAxis axis)
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
