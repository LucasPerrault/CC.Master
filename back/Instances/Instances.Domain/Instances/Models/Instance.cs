

namespace Instances.Domain.Instances.Models
{
	public class Instance
	{
		public int Id { get; set; }
		public InstanceType Type { get; set; }
		public string Cluster { get; set; }
		public bool IsProtected { get; set; }
		public bool IsAnonymized { get; set; }
		public bool IsActive { get; set; }
		public string AllUsersImposedPassword { get; set; }
		public int? EnvironmentId { get; set; }
	}
}
