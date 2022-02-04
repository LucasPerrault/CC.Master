using System;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Threading.Tasks;
using Billing.Contracts.Domain.Counts;
using Billing.Contracts.Domain.Counts.Services;

namespace Billing.Contracts.Web
{
    [ApiController, Route("/api/counts")]
    [ApiSort(nameof(Count.Id))]
    public class CountsController
    {
        private readonly IMissingCountsService _missingCountsService;

        public CountsController(IMissingCountsService missingCountsService)
        {
            _missingCountsService = missingCountsService;
        }

        [HttpGet("missing")]
        [ForbidIfMissing(Operation.ReadContracts)]
        public Task<Page<MissingCount>> GetMissingAsync([FromQuery] int Month, int Year)
        {
            return _missingCountsService.GetAsync(new DateTime(Year, Month, 01));
        }
    }
}
