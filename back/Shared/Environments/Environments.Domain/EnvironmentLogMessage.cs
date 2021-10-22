using Distributors.Domain.Models;
using System;

namespace Environments.Domain
{
    public class EnvironmentLogMessage
    {
        public int Id { get; set; }
        public string Name
        {
            get
            {
                return $"Message n°{this.Id}";
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int? UserId { get; set; }
        public int EnvironmentLogId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ExpiredOn { get; set; }
        public string Message { get; set; }
        public EnvironmentLogMessageTypes? Type
        {
            get
            {
                var isDefined = !string.IsNullOrWhiteSpace(TypeString);
                if (isDefined)
                {
                    return (EnvironmentLogMessageTypes)Enum.Parse(typeof(EnvironmentLogMessageTypes), TypeString);

                }
                return null;
            }
            set
            {
                TypeString = value.ToString();
            }
        }
        /// <summary>
        /// To modify this property, use Type property instead
        /// </summary>
        internal string TypeString { get; private set; }

    }
}
