using System;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Clients.Interfaces
{
    public interface IClientVisibilityService
    {
        Task<bool> IsClientVisibleInSubdomainAsync(Guid clientExternalId, string subdomain);
    }
}
