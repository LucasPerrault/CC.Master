using Instances.Application.CodeSources;
using Instances.Application.Instances;
using Instances.Domain.Github;
using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Web.Controllers
{
    [ApiController, Route("/api/branches")]
    public class BranchesController : ControllerBase
    {
        private readonly ICodeSourcesRepository _codeSourceRepository;
        private readonly IGithubBranchesRepository _githubBranchesRepository;

        public BranchesController(ICodeSourcesRepository codeSourcesRepository, IGithubBranchesRepository githubBranchesRepository)
        {
            _codeSourceRepository = codeSourcesRepository;
            _githubBranchesRepository = githubBranchesRepository;
        }


        [HttpPost("services/github-branch")]
        [ForbidIfMissing(Operation.EditGitHubBranchesAndPR)]
        public async Task CreateFromGithubAsync([FromBody] GithubBranchCreationDto githubBranchCreationDto)
        {
            var codeSources = await _codeSourceRepository.GetNonDeletedByRepositoryUrlAsync(githubBranchCreationDto.RepoUrl);
            if (!codeSources.Any())
            {
                throw new BadRequestException($"Url du repo invalide (aucune source de code trouvée) : {githubBranchCreationDto.RepoUrl}");
            }

            await _githubBranchesRepository.CreateAsync(codeSources, githubBranchCreationDto.BranchName);
        }
    }
}
