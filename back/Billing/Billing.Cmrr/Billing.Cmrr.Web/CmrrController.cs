using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain.Evolution;
using Billing.Cmrr.Domain.Situation;
using Billing.Cmrr.Infra.Services.Export;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Threading.Tasks;

namespace Billing.Cmrr.Web
{
    [ApiController, Route("/api/cmrr")]
    public class CmrrController
    {
        private readonly ICmrrSituationsService _cmrrSituationsService;
        private readonly ICmrrEvolutionsService _cmrrEvolutionsService;
        private readonly IFileExportService _csvService;

        public CmrrController
        (
            ICmrrSituationsService cmrrSituationService,
            ICmrrEvolutionsService cmrrEvolutionsService,
            IFileExportService csvService
        )
        {
            _cmrrSituationsService = cmrrSituationService;
            _cmrrEvolutionsService = cmrrEvolutionsService;
            _csvService = csvService;
        }

        [HttpGet("situation"), ForbidIfMissing(Operation.ReadCMRR)]
        public async Task<CmrrSituation> GetSituationAsync([FromQuery]CmrrQuery query)
        {
            var result = await _cmrrSituationsService.GetSituationAsync(query.ToCmrrFilter());
            return result;
        }

        [HttpPost("situation/export"), ForbidIfMissing(Operation.ReadCMRR)]
        public async Task<FileStreamResult> ExportSituationAsync([FromQuery] CmrrQuery query)
        {
            var situation = await _cmrrSituationsService.GetSituationAsync(query.ToCmrrFilter());

            var filename = $"cmrr-situation-{System.DateTime.Now:yyyyMMdd-HHmmss}.csv";
            return _csvService.Export(situation, filename);
        }

        [HttpGet("evolution"), ForbidIfMissing(Operation.ReadCMRR)]
        public async Task<CmrrEvolution> GetEvolutionAsync([FromQuery]CmrrQuery query)
        {
            var result = await _cmrrEvolutionsService.GetEvolutionAsync(query.ToCmrrFilter());
            return result;
        }
    }
}
