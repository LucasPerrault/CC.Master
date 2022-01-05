using Email.Domain;
using Lucca.Emails.Client;
using Lucca.Emails.Client.Contracts;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Email.Infra
{
    public class EmailService : IEmailService
    {
        private const string AppName = "Cloud Control";
        private readonly ILuccaEmailsClient _luccaEmails;

        public EmailService(ILuccaEmailsClient luccaEmails)
        {
            _luccaEmails = luccaEmails;
        }

        public Task SendAsync(RecipientForm recipientForm, EmailContent content)
        {
            return _luccaEmails.SendAsync
            (
                new NewEmail(ApplicationId.CC, AppName, content)
                {
                    RecipientId = GetRecipientId(recipientForm),
                    RecipientEmail = GetRecipientEmail(recipientForm),
                }
            );
        }

        private int? GetRecipientId(RecipientForm form)
        {
            return form.UserId;
        }

        private MailAddress GetRecipientEmail(RecipientForm recipientForm)
        {
            return recipientForm.Contact is null
                ? null
                : new MailAddress(recipientForm.Contact.EmailAddress);
        }
    }
}
