using Billing.Contracts.Domain.Contracts;
using System;
using System.Collections.Generic;

namespace Billing.Contracts.Domain.Clients
{
    public class Client
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Name { get; set; }
        public string SalesforceId { get; set; }
        public string BillingStreet { get; set; }
        public string BillingPostalCode { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingCountry { get; set; }
        public string BillingMail { get; set; }
        public string Phone { get; set; }
        public List<Contract> Contracts { get; set; }
    }
}
