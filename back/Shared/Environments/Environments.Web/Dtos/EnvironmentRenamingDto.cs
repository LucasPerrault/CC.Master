using Environments.Domain.ExtensionInterface;
using System.ComponentModel.DataAnnotations;

namespace Instances.Web.Dtos
{
    public class EnvironmentRenamingDto : IEnvironmentRenamingExtensionParameters
    {
        [Required]
        public int EnvironmentId { get; set; }
        [Required]
        public string NewName { get; set; }
        public bool HasRedirection { get; set; } = true;
    }
}
