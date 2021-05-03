using Email.Domain;
using Lucca.Emails.Client;
using Lucca.Emails.Client.Contracts;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Email.Infra
{
    public class EmailService : IEmailService
    {
        private const string NoReplyEmail = "no-reply@lucca.fr";
        private const string AppName = "Cloud Control";
        private readonly ILuccaEmailsClient _luccaEmails;

        public EmailService(ILuccaEmailsClient luccaEmails)
        {
            _luccaEmails = luccaEmails;
        }

        public Task SendAsync(SenderForm senderForm, RecipientForm recipientForm, EmailContent content)
        {
            return _luccaEmails.SendAsync
            (
                new NewEmail(ApplicationId.WEXTERNE, AppName)
                {
                    Content = content,
                    RecipientId = GetRecipientId(recipientForm),
                    RecipientEmail = GetRecipientEmail(recipientForm),
                    SenderEmail = GetSenderEmail(senderForm)
                }
            );
        }

        private int? GetRecipientId(RecipientForm form)
        {
            return form.UserId;
        }

        private MailAddress GetSenderEmail(SenderForm form)
        {
            return new MailAddress(NoReplyEmail, form.DisplayName);
        }

        private MailAddress GetRecipientEmail(RecipientForm recipientForm)
        {
            return recipientForm.Contact is null
                ? null
                : new MailAddress(recipientForm.Contact.EmailAddress);
        }
    }
}
