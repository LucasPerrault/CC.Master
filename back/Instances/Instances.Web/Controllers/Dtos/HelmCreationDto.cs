using System.ComponentModel.DataAnnotations;

namespace Instances.Web.Controllers.Dtos
{
    public class HelmCreationDto
    {
        [Required]
        public string ReleaseName { get; init; }
        [Required]
        public string HelmChart { get; init; }
        [Required]
        public string BranchName { get; init; }
    }
}
