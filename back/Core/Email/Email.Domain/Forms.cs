namespace Email.Domain
{
    public class RecipientForm
    {
        public int? UserId { get; private set; }
        public EmailContact Contact { get; private set; }

        private RecipientForm()
        { }

        public static RecipientForm FromUserId(int userId)
        {
            return new RecipientForm { UserId = userId };
        }

        public static RecipientForm FromContact(EmailContact contact)
        {
            return new RecipientForm { Contact = contact };
        }
    }

    public class SenderForm
    {
        public string DisplayName { get; set; }
    }

    public class EmailContact
    {
        public string EmailAddress { get; }

        private EmailContact(string emailAddress)
        {
            EmailAddress = emailAddress;
        }

        public static EmailContact CloudControl => new EmailContact("cc@ilucca.net");
    }
}
