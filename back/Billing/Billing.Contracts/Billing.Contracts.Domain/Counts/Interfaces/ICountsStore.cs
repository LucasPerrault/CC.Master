using Billing.Contracts.Domain.Counts.Filtering;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rights.Domain.Filtering;

namespace Billing.Contracts.Domain.Counts.Interfaces
{
    public interface ICountsStore
    {
        Task<IReadOnlyCollection<Count>> GetAsync(AccessRight right, CountFilter filter);
    }
}
