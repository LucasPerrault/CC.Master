using System;
using System.Threading.Tasks;

namespace Instances.Domain.Renaming
{
    public interface IRedirectionIisAdministration
    {
        Task CreateRedirectionAsync(string oldTenant, string newTenant, string domain, DateOnly endDate);
        Task BindDomainAsync(string newTenant, string domain);
    }
}
