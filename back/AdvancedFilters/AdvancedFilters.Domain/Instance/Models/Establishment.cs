using AdvancedFilters.Domain.Core.Models;
using System;
using Tools;

namespace AdvancedFilters.Domain.Instance.Models
{
    public class Establishment : IDeepCopyable<Establishment>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int LegalUnitId { get; set; }
        public string LegalIdentificationNumber { get; set; }
        public string ActivityCode { get; set; }
        public string TimeZoneId { get; set; }
        public int UsersCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsArchived { get; set; }
        public int EnvironmentId { get; set; }

        public LegalUnit LegalUnit { get; set; }
        public Environment Environment { get; set; }

        public Establishment DeepCopy()
        {
            return this.DeepCopyByExpressionTree();
        }
    }
}
