using Billing.Cmrr.Application;
using Billing.Cmrr.Application.Interfaces;
using Billing.Cmrr.Domain;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Domain.Abstractions;
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
        private readonly IRightsService _rightsService;

        public CmrrController(ICmrrSituationsService cmrrSituationService, IRightsService rightsService)
        {
            _cmrrSituationService = cmrrSituationService ?? throw new ArgumentNullException(nameof(cmrrSituationService));
            _rightsService = rightsService;
        }

        [HttpGet, ForbidIfMissing(Operation.ReadCMRR)]
        public async Task<List<CmrrContratSituation>> GetAsync([FromQuery] CmrrSituationFilter situationFilter)
        {
            var result = await _cmrrSituationService.GetContractSituationsAsync(situationFilter);
            return result;
        }
    }
}
