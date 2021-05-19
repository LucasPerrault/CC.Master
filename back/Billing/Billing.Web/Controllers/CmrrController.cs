using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System;
using System.Collections.Generic;
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
        public async Task<List<CmrrContratSituation>> GetAsync([FromQuery] CmrrSituationFilter situationFilter)
        {
            var result = await _cmrrSituationService.GetContractSituationsAsync(situationFilter);
            return result;
        }
    }
}
