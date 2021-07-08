using Instances.Application.CodeSources;
using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
using Instances.Web.Controllers.Dtos;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Instances.Web.Controllers
{
    [ApiController, Route("/api/code-sources")]
    [ApiSort("Code")]
    public class CodeSourcesController : ControllerBase
    {
        private readonly ICodeSourcesRepository _repository;

        public CodeSourcesController(ICodeSourcesRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.ReadCodeSources)]
        public async Task<Page<CodeSource>> GetAsync([FromQuery]CodeSourceQuery query)
        {
            return await _repository.GetAsync(query.Page, query.ToFilter());
        }

        [HttpGet("{id:int}")]
        [ForbidIfMissing(Operation.ReadCodeSources)]
        public async Task<CodeSource> GetAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        [HttpPost]
        [ForbidIfMissing(Operation.EditCodeSources)]
        public async Task<CodeSource> CreateAsync([FromBody]CodeSource codeSource)
        {
            return await _repository.CreateAsync(codeSource);
        }

        [HttpPut("{id:int}")]
        [ForbidIfMissing(Operation.EditCodeSources)]
        public async Task<CodeSource> UpdateAsync([FromRoute] int id, [FromBody]CodeSourceUpdate codeSourceUpdate)
        {
            return await _repository.UpdateAsync(id, codeSourceUpdate);
        }

        [HttpPost("update-production-version")]
        [ForbidIfMissing(Operation.EditCodeSources)]
        public async Task<ActionResult> UpdateProductionVersionAsync([FromBody]CodeSourceProductionVersionDto dto)
        {
            await _repository.UpdateProductionVersionAsync(dto);
            return new StatusCodeResult(StatusCodes.Status202Accepted);
        }

        [HttpPost("fetch-from-github")]
        [ForbidIfMissing(Operation.ReadCodeSources)]
        public async Task<Page<CodeSource>> FetchFromGithubAsync([FromBody]CodeSourceFetchDto dto)
        {
            var codeSources = await _repository.FetchFromRepoAsync(dto.RepoUrl);
            return new Page<CodeSource>
            {
                Count = codeSources.Count(),
                Items = codeSources
            };
        }

        [HttpPost("services/build-url")]
        [ForbidIfMissing(Operation.ReadCodeSources)]
        public async Task<IActionResult> GetBuildUrlAsync([FromBody] CodeSourceBuildUrlDto input)
        {
            if (input.BrancheName.Contains("/"))
            {
                return BadRequest("Invalid branchName number");
            }
            if (!string.IsNullOrEmpty(input.BuildNumber) && input.BuildNumber.Contains("/"))
            {
                return BadRequest("Invalid build number");
            }
            var buildUrl = await _repository.GetBuildUrlAsync(input.CodeSourceCode, input.BrancheName, input.BuildNumber);
            if (string.IsNullOrEmpty(buildUrl))
            {
                return NotFound();
            }
            return Ok(new CodeSourceBuildUrlResponse
            {
                Url = buildUrl
            });
        }

        [HttpGet("{id:int}/artifacts")]
        public async Task<List<CodeSourceArtifacts>> GetArtifactsListAsync(int id)
        {
            return await _repository.GetArtifactsAsync(id);
        }
    }

    public class CodeSourceQuery
    {
        public IPageToken Page { get; set; } = null;
        public HashSet<CodeSourceLifecycleStep> Lifecycle { get; set; } = CodeSource.ActiveSteps;
        public HashSet<CodeSourceType> Type { get; set; } = new HashSet<CodeSourceType>();

        public string Search { get; set; }

        public HashSet<int> Id { get; set; } = new HashSet<int>();

        public string Code { get; set; }

        public CodeSourceFilter ToFilter() => new CodeSourceFilter
        {
            Code = Code,
            Id = Id,
            Search = Search,
            Lifecycle = Lifecycle,
            Type = Type
        };
    }
}
