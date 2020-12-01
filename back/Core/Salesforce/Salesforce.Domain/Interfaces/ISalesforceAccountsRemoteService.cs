using Salesforce.Domain.Models;
using System.Threading.Tasks;

namespace Salesforce.Domain.Interfaces
{
    public interface ISalesforceAccountsRemoteService
    {
        Task UpdateAccountAsync(string clientSalesforceId, SalesforceAccount account);
    }
}
