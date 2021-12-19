using Lucca.Emails.Client.Contracts;

namespace Billing.Contracts.Infra.Services
{
    public static class CountCacheEmail
    {
        public static EmailTemplate Template()
        {
            return new EmailTemplate();
        }
    }
}
