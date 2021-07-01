using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using Billing.Cmrr.Domain.Evolution;
using Billing.Cmrr.Domain.Situation;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Threading.Tasks;

namespace Billing.Web.Controllers
{
    [ApiController, Route("/api/cmrr")]
    public class CmrrController
    {
        private readonly ICmrrSituationsService _cmrrSituationsService;
        private readonly ICmrrEvolutionsService _cmrrEvolutionsService;

        public CmrrController(ICmrrSituationsService cmrrSituationService, ICmrrEvolutionsService cmrrEvolutionsService)
        {
            _cmrrSituationsService = cmrrSituationService;
            _cmrrEvolutionsService = cmrrEvolutionsService;
        }

        [HttpGet("situation"), ForbidIfMissing(Operation.ReadCMRR)]
        public async Task<CmrrSituation> GetSituationAsync([FromQuery] CmrrSituationListQuery situationListQuery)
        {
            var result = await _cmrrSituationsService.GetSituationAsync(situationListQuery.ToCmrrSituationFilter());
            return result;
        }

        [HttpGet("evolution"), ForbidIfMissing(Operation.ReadCMRR)]
        public async Task<CmrrEvolution> GetEvolutionAsync([FromQuery] CmrrEvolutionListQuery evolutionListQuery)
        {
            var result = await _cmrrEvolutionsService.GetEvolutionAsync(evolutionListQuery.ToCmrrEvolutionFilter());
            return result;
        }
    }
}
