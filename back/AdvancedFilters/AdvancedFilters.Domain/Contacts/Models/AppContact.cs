using AdvancedFilters.Domain.Instance.Models;
using System;

namespace AdvancedFilters.Domain.Contacts.Models
{
    public class AppContact
    {
        public int Id { get; set; }
        public int RemoteId { get; set; }
        public int AppInstanceId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsConfirmed { get; set; }

        public AppInstance AppInstance { get; set; }
    }
}
