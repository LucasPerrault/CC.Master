namespace Salesforce.Domain.Models
{
    public class SalesforceAccount
    {
        public string Id { get; set; }
        public string BillingStreet { get; set; }
        public string BillingCity { get; set; }
        public string BillingPostalCode { get; set; }
        public string BillingCountry { get; set; }
        public string Email_de_facturation__c { get; set; }
        public string Phone { get; set; }
    }
}
