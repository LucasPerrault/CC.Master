using Billing.Cmrr.Domain;
using Billing.Products.Domain;
using Billing.Products.Domain.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Cmrr.Application
{
    public interface IBreakdownService
    {
        Task<List<Breakdown>> GetAllBreakdownsAsync(CmrrAxis axis);
    }

    public class BreakdownService : IBreakdownService
    {
        private readonly IProductsStore _productsStore;

        public BreakdownService(IProductsStore productsStore)
        {
            _productsStore = productsStore;
        }

        public async Task<List<Breakdown>> GetAllBreakdownsAsync(CmrrAxis axis)
        {
            var products = await _productsStore.GetNonFreeProductsAsync();

            return products
                .SelectMany(p => p.ProductSolutions)
                .Select(ps => ToBreakdownShare(axis, ps))
                .GroupBy(share => share.Product.Id)
                .SelectMany(shares => ToBreakdowns(shares.ToList()))
                .ToList();
        }

        private BreakdownShare ToBreakdownShare(CmrrAxis axis, ProductSolution ps)
        {
            return axis switch
            {
                CmrrAxis.BusinessUnit => new BreakdownShare
                (
                    new AxisSection { Id = ps.Solution.BusinessUnitId, Name = ps.Solution.BusinessUnit.Name },
                    ps.Product.Name,
                    ps.Product,
                    ps.Share
                ),
                CmrrAxis.Product =>  new BreakdownShare
                (
                    new AxisSection { Id = ps.Product.FamilyId, Name = ps.Product.Family.Name },
                    ps.Solution.Name,
                    ps.Product,
                    ps.Share
                ),
                _ => throw new InvalidEnumArgumentException(nameof(axis), (int)axis, typeof(CmrrAxis))
            };
        }

        private IEnumerable<Breakdown> ToBreakdowns(List<BreakdownShare> shares)
        {
            decimal sum = shares.Sum(s => s.Share);
            var ratioPartialSum = 0m;

            for (var i = 0; i < shares.Count - 1; i++)
            {
                var breakdownShare = shares[i];
                var ratio = breakdownShare.Share / sum;
                ratioPartialSum += ratio;
                yield return new Breakdown
                {
                    AxisSection = breakdownShare.AxisSection,
                    Ratio = ratio,
                    ProductId = breakdownShare.Product.Id,
                    SubSection = breakdownShare.SubSection
                };
            }

            var lastBreakdownShare = shares.Last();

            yield return new Breakdown
            {
                AxisSection = lastBreakdownShare.AxisSection,
                Ratio = 1 - ratioPartialSum,
                ProductId = lastBreakdownShare.Product.Id,
                SubSection = lastBreakdownShare.SubSection
            };
        }

        private class BreakdownShare
        {
            public AxisSection AxisSection { get; }

            public int Share { get; }

            public string SubSection { get; }
            public Product Product { get; }

            public BreakdownShare(AxisSection axisSection, string subSection, Product product, int share)
            {
                AxisSection = axisSection;
                Share = share;
                SubSection = subSection;
                Product = product;
            }
        }
    }
}
