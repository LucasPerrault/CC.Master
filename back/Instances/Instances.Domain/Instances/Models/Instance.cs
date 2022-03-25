

namespace Instances.Domain.Instances.Models
{
	public class Instance
	{
        public const string TRAINING_EXECUTING_CLUSTER = "TEST";
        public const string PREVIEW_EXECUTING_CLUSTER = "PREVIEW";

        public int Id { get; set; }
		public InstanceType Type { get; set; }
		public bool IsProtected { get; set; }
		public bool IsAnonymized { get; set; }
		public bool IsActive { get; set; }
		public string AllUsersImposedPassword { get; set; }
		public int? EnvironmentId { get; set; }
	}
}
