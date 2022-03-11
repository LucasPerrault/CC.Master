using System;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Threading.Tasks;
using Billing.Contracts.Application.Counts;
using Billing.Contracts.Domain.Counts;
using Lucca.Core.Shared.Domain.Exceptions;

namespace Billing.Contracts.Web
{
    [ApiController, Route("/api/counts")]
    [ApiSort(nameof(Count.Id))]
    public class CountsController
    {
        private readonly CountsRepository _countsRepository;

        public CountsController(CountsRepository countsRepository)
        {
            _countsRepository = countsRepository;
        }

        [HttpGet("missing")]
        [ForbidIfMissing(Operation.ReadContracts)]
        public async Task<Page<MissingCount>> GetMissingCountsAsync([FromQuery] MissingCountsQuery query)
        {
            if (!query.Period.HasValue)
            {
                throw new BadRequestException($"{nameof(MissingCountsQuery.Period)} query param is mandatory");
            }
            var missingCounts = await _countsRepository.GetMissingCountsAsync(query.Period.Value);
            return new Page<MissingCount> { Count = missingCounts.Count, Items = missingCounts };

        }

        public class MissingCountsQuery
        {
            public DateTime? Period { get; set; }
        }
    }
}
