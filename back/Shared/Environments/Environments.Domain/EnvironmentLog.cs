using Distributors.Domain.Models;
using System;
using System.Collections.Generic;
using Tools;

namespace Environments.Domain
{
    public class EnvironmentLog
    {
        public EnvironmentLog()
        {
            Messages = new List<EnvironmentLogMessage>();
        }
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EnvironmentId { get; set; }
        public EnvironmentLogActivity ActivityId { get; set; }
        public string Activity => EnumExtensions.GetDescription(ActivityId);
        public DateTime CreatedOn { get; set; }
        public bool IsAnonymizedData { get; set; }

        public ICollection<EnvironmentLogMessage> Messages { get; set; }
    }
}
