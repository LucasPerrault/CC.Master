using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System;
using System.Threading.Tasks;

namespace Billing.Web.Controllers
{
    [ApiController, Route("/api/cmrr")]
    public class CmrrController
    {
        private readonly ICmrrSituationsService _cmrrSituationService;

        public CmrrController(ICmrrSituationsService cmrrSituationService)
        {
            _cmrrSituationService = cmrrSituationService ?? throw new ArgumentNullException(nameof(cmrrSituationService));
        }

        [HttpGet, ForbidIfMissing(Operation.ReadCMRR)]
        public async Task<CmrrSituation> GetAsync([FromQuery] CmrrSituationListQuery situationListQuery)
        {
            var result = await _cmrrSituationService.GetSituationAsync(situationListQuery.ToCmrrSituationFilter());
            return result;
        }
    }
}
