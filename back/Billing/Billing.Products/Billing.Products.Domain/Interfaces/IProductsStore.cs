using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Products.Domain.Interfaces
{
    public interface IProductsStore
    {
        Task<List<Product>> GetProductsAsync();
    }
}
