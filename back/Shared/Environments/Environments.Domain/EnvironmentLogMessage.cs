using System;

namespace Environments.Domain
{
    public class EnvironmentLogMessage
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int EnvironmentLogId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ExpiredOn { get; set; }
        public string Message { get; set; }
        public EnvironmentLogMessageTypes Type { get; set; }
    }
}
