using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Situation;
using System;
using System.Collections.Generic;

namespace Billing.Cmrr.Application
{


    public class CmrrFilter
    {
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }

        public CmrrAxis Axis { get; set; } = CmrrAxis.Product;

        public HashSet<int> ClientId { get; set; } = new HashSet<int>();
        public HashSet<int> DistributorsId { get; set; } = new HashSet<int>();
        public HashSet<BillingStrategy> BillingStrategies { get; set; } = new HashSet<BillingStrategy>();
        public HashSet<string> Sections { get; set; } = new HashSet<string>();
    }
}
