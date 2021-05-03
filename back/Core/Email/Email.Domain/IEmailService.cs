using Lucca.Emails.Client.Contracts;
using System.Threading.Tasks;

namespace Email.Domain
{
    public interface IEmailService
    {
        Task SendAsync(SenderForm senderForm, RecipientForm form, EmailContent content);
    }
}
