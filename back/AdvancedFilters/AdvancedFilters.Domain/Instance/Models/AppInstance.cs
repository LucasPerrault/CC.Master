using System;

namespace AdvancedFilters.Domain.Instance.Models
{
    public class AppInstance
    {
        public int Id { get; set; }
        public int RemoteId { get; set; }
        public string Name { get; set; }
        public string ApplicationId { get; set; }
        public int EnvironmentId { get; set; }
        public DateTime? DeleteAt { get; set; }

        public Environment Environment { get; set; }
    }
}
