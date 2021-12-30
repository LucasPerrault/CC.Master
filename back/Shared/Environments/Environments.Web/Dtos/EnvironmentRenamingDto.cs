using System.ComponentModel.DataAnnotations;

namespace Instances.Web.Dtos
{
    public class EnvironmentRenamingDto
    {
        [Required]
        public int EnvironmentId { get; set; }
        [Required]
        public string NewName { get; set; }
    }
}
