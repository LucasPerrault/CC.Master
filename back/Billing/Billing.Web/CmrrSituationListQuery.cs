using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using System;
using System.Collections.Generic;

namespace Billing.Web
{
    public class CmrrSituationListQuery
    {
        public DateTime? StartPeriod { get; set; }
        public DateTime? EndPeriod { get; set; }

        public CmrrAxis? Axis { get; set; }

        public HashSet<int> ClientId { get; set; } = new HashSet<int>();
        public HashSet<string> DistributorsId { get; set; } = new HashSet<string>();

        public CmrrSituationFilter ToCmrrSituationFilter()
        {
            if (StartPeriod is null)
                throw new ArgumentNullException($"{nameof(StartPeriod)} must be specified");
            if (EndPeriod is null)
                throw new ArgumentNullException($"{nameof(EndPeriod)} must be specified");

            return new CmrrSituationFilter
            {
                StartPeriod = StartPeriod.Value,
                EndPeriod = EndPeriod.Value,
                Axis = Axis ?? CmrrAxis.Product,
                ClientId = ClientId,
                DistributorsId = DistributorsId
            };
        }
    }
}
