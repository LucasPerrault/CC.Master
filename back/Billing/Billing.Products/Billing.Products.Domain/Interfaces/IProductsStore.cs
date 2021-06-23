using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Products.Domain.Interfaces
{
    public class ProductsFilter
    {
        public bool NonFreeOnly { get; set; }
        public static ProductsFilter All = new ProductsFilter();
    }

    public class ProductsIncludes
    {
        public bool Families { get; set; }
        public bool Solutions { get; set; }
        public bool BusinessUnits { get; set; }


        public static ProductsIncludes All = new ProductsIncludes { Families = true, BusinessUnits = true};
    }

    public interface IProductsStore
    {
        Task<List<Product>> GetAsync(ProductsFilter filter, ProductsIncludes includes);
    }
}
