using Instances.Application.Instances;
using Instances.Application.Instances.Dtos;
using Instances.Web.Controllers.Dtos;
using Instances.Web.Controllers.Query;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Collections.Generic;
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
        [ForbidIfMissing(Operation.EditGitHubBranchesAndPR)]
        public Task CreateHelmAsync([FromBody] HelmCreationDto helmCreationDto)
            => _helmRepository.CreateHelmAsync(
                helmCreationDto.ReleaseName,
                helmCreationDto.BranchName,
                helmCreationDto.HelmChart
            );

        [HttpGet("releases")]
        [ForbidIfMissing(Operation.ReadCodeSources)]
        public Task<List<HelmRelease>> GetAsync([FromQuery] HelmReleaseQuery query)
        {
            if (query.LastStable && !string.IsNullOrEmpty(query.GitRef))
            {
                throw new BadRequestException($"{nameof(query.LastStable)} and {nameof(query.GitRef)} could not be defined at the same time");
            }

            return _helmRepository.GetAllReleasesAsync(query.ReleaseName, query.GitRef, query.LastStable);
        }
    }
}
