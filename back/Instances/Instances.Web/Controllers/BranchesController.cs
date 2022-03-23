using Instances.Application.Instances;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Threading.Tasks;

namespace Instances.Web.Controllers
{
    [ApiController, Route("/api/branches")]
    public class BranchesController : ControllerBase
    {
        private readonly IGithubBranchesRepository _githubBranchesRepository;

        public BranchesController(IGithubBranchesRepository githubBranchesRepository)
        {
            _githubBranchesRepository = githubBranchesRepository;
        }


        [HttpPost("services/github-branch")]
        [ForbidIfMissing(Operation.EditGitHubBranchesAndPR)]
        public async Task CreateFromGithubAsync([FromBody] GithubBranchCreationDto githubBranchCreationDto)
        {
            await _githubBranchesRepository.CreateAsync(githubBranchCreationDto.RepoId, githubBranchCreationDto.BranchName);
        }
    }
}
