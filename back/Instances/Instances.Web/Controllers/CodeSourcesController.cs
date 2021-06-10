using Instances.Application.CodeSources;
using Instances.Domain.CodeSources;
using Instances.Domain.CodeSources.Filtering;
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
    public class CodeSourcesController
    {
        private readonly CodeSourcesAppController _appController;

        public CodeSourcesController(CodeSourcesAppController appController)
        {
            _appController = appController;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.ReadCodeSources)]
        public async Task<FetchedCodeSources> GetAsync([FromQuery]CodeSourceQuery query)
        {
            var codeSources = await _appController.GetAsync(query.ToFilter());
            return new FetchedCodeSources { CodeSources = codeSources.ToList() };
        }

        [HttpGet("{id:int}")]
        [ForbidIfMissing(Operation.ReadCodeSources)]
        public async Task<FetchedCodeSources> GetAsync(int id)
        {
            var codeSources = await _appController.GetAsync(CodeSourceFilter.ById(id));
            return new FetchedCodeSources { CodeSources = codeSources.ToList() };
        }

        [HttpPost]
        [ForbidIfMissing(Operation.EditCodeSources)]
        public async Task<CodeSource> CreateAsync([FromBody]CodeSource codeSource)
        {
            return await _appController.CreateAsync(codeSource);
        }

        [HttpPut("{id:int}")]
        [ForbidIfMissing(Operation.EditCodeSources)]
        public async Task<CodeSource> UpdateAsync([FromRoute] int id, [FromBody]CodeSourceUpdate codeSourceUpdate)
        {
            return await _appController.UpdateAsync(id, codeSourceUpdate);
        }

        [HttpPost("update-production-version")]
        [ForbidIfMissing(Operation.EditCodeSources)]
        public async Task<ActionResult> UpdateProductionVersionAsync([FromBody]CodeSourceProductionVersionDto codeSourceProductionVersionDto)
        {
            await _appController.UpdateProductionVersionAsync(codeSourceProductionVersionDto);
            return new StatusCodeResult(StatusCodes.Status202Accepted);
        }

        [HttpPost("fetch-from-github")]
        [ForbidIfMissing(Operation.ReadCodeSources)]
        public async Task<FetchedCodeSources> FetchFromGithubAsync([FromBody]CodeSourceFetchDto dto)
        {
            var codeSources = await _appController.FetchFromRepoAsync(dto.RepoUrl);
            return new FetchedCodeSources { CodeSources = codeSources.ToList() };
        }

        public class CodeSourceFetchDto
        {
            public string RepoUrl { get; set; }
        }

        public class FetchedCodeSources
        {
            public List<CodeSource> CodeSources { get; set; }
        }
    }

    public class CodeSourceQuery
    {
        public CodeSourceFilter ToFilter() => new CodeSourceFilter
        {
            Code = Code,
            Id = Id,
            Search = Search,
            Lifecycle = Lifecycle
        };

        public HashSet<CodeSourceLifecycleStep> Lifecycle { get; set; } = CodeSource.ActiveSteps;

        public string Search { get; set; }

        public HashSet<int> Id { get; set; }

        public string Code { get; set; }
    }
}
