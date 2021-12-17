using System;
using System.Collections.Generic;
using Tools;

namespace IpFilter.Domain
{

    public class IpFilterAuthorizationRequest
    {
        public int Id { get; set; }
        public Guid Code { get; set; }
        public int UserId { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
    }

    public class IpFilterAuthorizationRequestFilter
    {
        public int? UserId { get; set; }
        public HashSet<string> Addresses { get; set; } = new HashSet<string>();
        public CompareDateTime CreatedAt { get; set; } = CompareDateTime.Bypass();
        public CompareDateTime ExpiresAt { get; set; } = CompareDateTime.Bypass();
        public CompareNullableDateTime RevokedAt { get; set; } = CompareNullableDateTime.Bypass();
        public Guid? Code { get; set; }
    }
}
