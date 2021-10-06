using AdvancedFilters.Domain.Core.Models;
using AdvancedFilters.Domain.Instance.Models;
using System;
using Tools;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Domain.Contacts.Models
{
    public class AppContact : IDeepCopyable<AppContact>
    {
        public int Id { get; set; }
        public int AppInstanceId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsConfirmed { get; set; }
        public int EnvironmentId { get; set; }
        public int EstablishmentId { get; set; }

        public AppInstance AppInstance { get; set; }
        public Environment Environment { get; set; }
        public Establishment Establishment { get; set; }

        public AppContact DeepCopy()
        {
            return this.DeepCopyByExpressionTree();
        }
    }
}
