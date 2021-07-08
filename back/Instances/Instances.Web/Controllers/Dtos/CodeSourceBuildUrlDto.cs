using System.ComponentModel.DataAnnotations;

namespace Instances.Web.Controllers.Dtos
{
    public class CodeSourceBuildUrlDto
    {
        [Required]
        public string CodeSourceCode { get; set; }
        [Required]
        public string BrancheName { get; set; }
        public string BuildNumber { get; set; } 
    }

    public class CodeSourceBuildUrlResponse
    {
        public string Url { get; set; }
    }
}
