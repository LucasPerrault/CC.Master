using System;
using Tools;

namespace IpFilter.Domain
{
    public enum AuthorizationDuration
    {
        OneDay = 0,
        SixMonth = 1,
    }

    public class IpFilterAuthorization
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string IpAddress { get; set; }
        public string Device { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int? RequestId { get; set; }

        public IpFilterAuthorizationRequest Request { get; set; }
    }

    public class IpFilterAuthorizationFilter
    {
        public int? UserId { get; set; }
        public int? RequestId { get; set; }

        public string IpAddress { get; set; }
        public CompareDateTime CreatedAt { get; set; } = CompareDateTime.Bypass();
        public CompareDateTime ExpiresAt { get; set; } = CompareDateTime.Bypass();
    }
}
