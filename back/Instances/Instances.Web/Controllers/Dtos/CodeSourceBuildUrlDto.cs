using System.ComponentModel.DataAnnotations;

namespace Instances.Web.Controllers.Dtos
{
    public class CodeSourceBuildUrlDto
    {
        [Required]
        public string CodeSourceCode { get; set; }
        public string BrancheName { get; set; }
        [Required]
        public string BuildNumber { get; set; } 
    }

    public class CodeSourceBuildUrlResponse
    {
        public string Url { get; set; }
    }
}
