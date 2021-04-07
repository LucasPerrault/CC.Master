using System.Threading.Tasks;

namespace Instances.Domain.Demos
{
    public interface IHubspotService
    {
        Task<HubspotContact> GetContactAsync(int vId);
        Task UpdateContactSubdomainAsync(int vId, string subdomain);
        Task CallWorkflowForEmailAsync(int workflowId, string email);
    }
}