using Billing.Contracts.Domain.Counts.Filtering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Counts.Interfaces
{
    public interface ICountsStore
    {
        Task<IReadOnlyCollection<Count>> GetAsync(CountFilter filter);
    }
}
