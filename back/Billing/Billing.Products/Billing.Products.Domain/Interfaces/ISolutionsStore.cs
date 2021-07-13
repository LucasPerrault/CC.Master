using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Products.Domain.Interfaces
{
    public interface ISolutionsStore
    {
        Task<List<Solution>> GetAsync();
    }
}
