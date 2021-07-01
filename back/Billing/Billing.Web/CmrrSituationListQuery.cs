using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Situation;
using System;
using System.Collections.Generic;

namespace Billing.Web
{
    public class CmrrSituationListQuery
    {
        public DateTime? StartPeriod { get; set; }
        public DateTime? EndPeriod { get; set; }

        public CmrrAxis? Axis { get; set; }
        public HashSet<string> Sections { get; set; } = new HashSet<string>();

        public HashSet<int> ClientId { get; set; } = new HashSet<int>();
        public HashSet<string> DistributorId { get; set; } = new HashSet<string>();
        public HashSet<BillingStrategy> BillingStrategy { get; set; } = new HashSet<BillingStrategy>();

        public CmrrSituationFilter ToCmrrSituationFilter()
        {
            if (StartPeriod is null)
                throw new ArgumentNullException($"{nameof(StartPeriod)} must be specified");
            if (EndPeriod is null)
                throw new ArgumentNullException($"{nameof(EndPeriod)} must be specified");

            var sections = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
            foreach (var section in Sections)
            {
                sections.Add(section);
            }

            return new CmrrSituationFilter
            {
                StartPeriod = StartPeriod.Value,
                EndPeriod = EndPeriod.Value,
                Axis = Axis ?? CmrrAxis.Product,
                ClientId = ClientId,
                DistributorsId = DistributorId,
                BillingStrategies = BillingStrategy,
                Sections = sections
            };
        }
    }
}
