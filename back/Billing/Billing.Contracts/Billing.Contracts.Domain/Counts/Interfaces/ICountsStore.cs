using Billing.Contracts.Domain.Counts.Filtering;
using System.Linq;

namespace Billing.Contracts.Domain.Counts.Interfaces
{
    public interface ICountsStore
    {
        IQueryable<Count> GetQueryable(CountFilter filter);
    }
}
