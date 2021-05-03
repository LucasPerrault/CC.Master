using System;

namespace Instances.Domain.Demos.Cleanup
{
    public interface IDemoDeletionCalculator
    {
        DateTime GetDeletionDate(Demo demo, DateTime lastUsedAt);
    }

    public class DemoDeletionCalculator : IDemoDeletionCalculator
    {
        private const int HubspotUserId = 0;

        private static readonly TimeSpan StandardAcceptableInactivity = TimeSpan.FromDays(62);
        private static readonly TimeSpan HubspotAcceptableInactivity = TimeSpan.FromDays(31);

        public DateTime GetDeletionDate(Demo demo, DateTime lastUsedAt)
        {
            return lastUsedAt.Add(GetAcceptableInactivity(demo));
        }

        private TimeSpan GetAcceptableInactivity(Demo demo)
        {
            return demo.AuthorId == HubspotUserId
                ? HubspotAcceptableInactivity
                : StandardAcceptableInactivity;
        }
    }
}
