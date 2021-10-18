using Instances.Application.Instances;
using Instances.Web.Controllers.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Instances.Web.Controllers
{
    [ApiController, Route("/api/helm")]
    public class HelmController
    {
        private readonly IHelmRepository _helmRepository;

        public HelmController(IHelmRepository helmRepository)
        {
            _helmRepository = helmRepository;
        }

        [HttpPost]
        public Task CreateHelmAsync([FromBody] HelmCreationDto helmCreationDto)
            => _helmRepository.CreateHelmAsync(
                helmCreationDto.ReleaseName,
                helmCreationDto.BranchName,
                helmCreationDto.HelmChart
            );

    }
}
