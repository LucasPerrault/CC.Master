using System.Collections.Generic;
using System.Linq;

namespace Billing.Cmrr.Domain.Situation
{
    public class CmrrClientsSituationService
    {
        public CmrrClientSituation GetClientsSituation(List<CmrrContractSituation> contractSituations)
        {
            var acquired = new List<CmrrClient>();
            var terminated = new List<CmrrClient>();

            foreach (var group in contractSituations.GroupBy(s => s.Contract.ClientId))
            {
                if (group.All(c => c.StartPeriodCount == null))
                {
                    acquired.Add(new CmrrClient { Name = group.First().Contract.ClientName });
                }
                if (group.All(c => c.EndPeriodCount == null))
                {
                    terminated.Add(new CmrrClient { Name = group.First().Contract.ClientName });
                }
            }

            return new CmrrClientSituation
            {
                Terminated = terminated,
                Acquired = acquired
            };
        }
    }
}
