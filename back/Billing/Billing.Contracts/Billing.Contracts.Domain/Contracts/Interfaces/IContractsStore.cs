using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Contracts.Interfaces
{
    public interface IContractsStore
    {
        Task<IReadOnlyCollection<Contract>> GetAsync(Guid clientExternalId, string subdomain);
    }
}
