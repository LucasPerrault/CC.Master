using Billing.Cmrr.Domain.Situation;
using Billing.Products.Domain;
using Billing.Products.Domain.Interfaces;
using Cache.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Billing.Cmrr.Application
{
    public interface IBreakdownService
    {
        Task<List<Breakdown>> GetBreakdownsAsync(CmrrAxis axis);
    }

    public class BreakDownInMemoryCache : InMemoryCache<CmrrAxis, List<Breakdown>>
    {
        public BreakDownInMemoryCache() : base(TimeSpan.FromMinutes(1))
        { }
    }

    public class BreakdownService : IBreakdownService
    {
        private readonly IProductsStore _productsStore;
        private readonly BreakDownInMemoryCache _cache;

        public BreakdownService(IProductsStore productsStore, BreakDownInMemoryCache cache)
        {
            _productsStore = productsStore;
            _cache = cache;
        }

        public async Task<List<Breakdown>> GetBreakdownsAsync(CmrrAxis axis)
        {
            if (_cache.TryGet(axis, out List<Breakdown> cachedBreakdowns))
            {
                return cachedBreakdowns;
            }

            var breakdowns = await ComputeBreakdownsAsync(axis);
            _cache.Cache(axis, breakdowns);
            return breakdowns;
        }

        private async Task<List<Breakdown>> ComputeBreakdownsAsync(CmrrAxis axis)
        {
            var products = await _productsStore.GetAsync(new ProductsFilter { NonFreeOnly = true}, ProductsIncludes.All);

            return products
                .Select(p => p.ProductSolutions.Select(ps => ToBreakdownShare(axis, ps)))
                .SelectMany(shares => ToBreakdowns(shares.ToList()))
                .ToList();
        }

        private BreakdownShare ToBreakdownShare(CmrrAxis axis, ProductSolution ps)
        {
            return axis switch
            {
                CmrrAxis.BusinessUnit => new BreakdownShare
                (
                    AxisSection.ForBusinessUnit(ps.Solution.BusinessUnit),
                    ps.Product.Name,
                    ps.Product,
                    ps.Share
                ),
                CmrrAxis.Product =>  new BreakdownShare
                (
                    AxisSection.ForProductFamily(ps.Product.Family),
                    ps.Product.Name,
                    ps.Product,
                    ps.Share
                ),
                CmrrAxis.Solution => new BreakdownShare
                (
                    AxisSection.ForSolution(ps.Solution),
                    ps.Product.Name,
                    ps.Product,
                    ps.Share
                ),
                _ => throw new InvalidEnumArgumentException(nameof(axis), (int)axis, typeof(CmrrAxis))
            };
        }

        private IEnumerable<Breakdown> ToBreakdowns(List<BreakdownShare> shares)
        {

            if (shares.Count == 1)
            {
                var share = shares.Single();
                yield return new Breakdown
                {
                    Ratio = 1,
                    AxisSection = share.AxisSection,
                    ProductId = share.Product.Id,
                    SubSection = share.SubSection
                };
                yield break;
            }

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
