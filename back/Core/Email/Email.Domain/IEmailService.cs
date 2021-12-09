using Lucca.Emails.Client.Contracts;
using System.Threading.Tasks;

namespace Email.Domain
{
    public interface IEmailService
    {
        Task SendAsync(RecipientForm form, EmailContent content);
    }
}
