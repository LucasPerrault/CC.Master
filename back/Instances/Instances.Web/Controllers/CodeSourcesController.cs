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
        private readonly CodeSourcesRepository _repository;

        public CodeSourcesController(CodeSourcesRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ForbidIfMissing(Operation.ReadCodeSources)]
        public async Task<CodeSourceItems> GetAsync([FromQuery]CodeSourceQuery query)
        {
            var codeSources = await _repository.GetAsync(query.ToFilter());
            return new CodeSourceItems { Items = codeSources.ToList() };
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
        public async Task<CodeSourceItems> FetchFromGithubAsync([FromBody]CodeSourceFetchDto dto)
        {
            var codeSources = await _repository.FetchFromRepoAsync(dto.RepoUrl);
            return new CodeSourceItems { Items = codeSources.ToList() };
        }

        public class CodeSourceFetchDto
        {
            public string RepoUrl { get; set; }
        }

        public class CodeSourceItems
        {
            public List<CodeSource> Items { get; set; }
        }
    }

    public class CodeSourceQuery
    {

        public HashSet<CodeSourceLifecycleStep> Lifecycle { get; set; } = CodeSource.ActiveSteps;

        public string Search { get; set; }

        public HashSet<int> Id { get; set; } = new HashSet<int>();

        public string Code { get; set; }

        public CodeSourceFilter ToFilter() => new CodeSourceFilter
        {
            Code = Code,
            Id = Id,
            Search = Search,
            Lifecycle = Lifecycle
        };
    }
}
