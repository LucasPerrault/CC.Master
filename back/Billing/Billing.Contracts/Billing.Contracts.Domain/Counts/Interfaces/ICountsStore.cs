using Billing.Contracts.Domain.Counts.Filtering;
using Rights.Domain.Filtering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Counts.Interfaces
{
    public interface ICountsStore
    {
        Task<IReadOnlyCollection<Count>> GetAsync(AccessRight access, CountFilter filter);
    }
}
