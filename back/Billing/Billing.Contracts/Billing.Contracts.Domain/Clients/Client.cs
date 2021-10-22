using Billing.Contracts.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Billing.Contracts.Domain.Clients
{
    public class Client
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Name { get; set; }
        public string SocialReason { get; set; }
        public string SalesforceId { get; set; }
        public string BillingStreet { get; set; }
        public string BillingPostalCode { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingCountry { get; set; }
        public string BillingMail { get; set; }
        public string Phone { get; set; }

        [JsonIgnore]
        public List<Contract> Contracts { get; set; }
    }
}
