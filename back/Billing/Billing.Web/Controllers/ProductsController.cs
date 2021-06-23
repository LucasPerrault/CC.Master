using Billing.Products.Domain;
using Billing.Products.Domain.Interfaces;
using Lucca.Core.Api.Abstractions.Paging;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Threading.Tasks;

namespace Billing.Web.Controllers
{
    [ApiController, Route("/api/products")]
    public class ProductsController
    {
        private readonly IProductsStore _productsStore;

        public ProductsController(IProductsStore productsStore)
        {
            _productsStore = productsStore;
        }

        [HttpGet, ForbidIfMissing(Operation.ReadCMRR)]
        public async Task<Page<Product>> GetAsync()
        {
            var result = await _productsStore.GetAsync(ProductsFilter.All, new ProductsIncludes { Families = true });
            return new Page<Product>
            {
                Items = result,
                Count = result.Count,
                Next = null,
                Prev = null
            };
        }
    }
}
