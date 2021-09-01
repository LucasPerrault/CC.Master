using AdvancedFilters.Domain.Billing.Models;
using System;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Domain.Contacts.Models
{
    public class ClientContact
    {
        public int Id { get; set; }
        public int RemoteId { get; set; }
        public int RoleId { get; set; }
        public Guid ClientId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsConfirmed { get; set; }
        public int EnvironmentId { get; set; }

        public Client Client { get; set; }
        public Environment Environment { get; set; }
    }
}
