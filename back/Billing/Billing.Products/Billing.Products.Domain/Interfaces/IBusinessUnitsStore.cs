using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Products.Domain.Interfaces
{
    public interface IBusinessUnitsStore
    {
        Task<List<BusinessUnit>> GetAsync();
    }
}
