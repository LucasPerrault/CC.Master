using System;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Web.ModelBinding.Sorting;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Threading.Tasks;
using Billing.Contracts.Application.Counts;
using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Counts;

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
        public Task<Page<MissingCount>> GetMissingAsync([FromQuery] MissingCountQuery query)
        {
            return _countsRepository.GetMissingCountsAsync(query.ToAccountPeriod());
        }

        public class MissingCountQuery
        {
            public int? Month { get; set; }
            public int? Year { get; set; }

            public AccountingPeriod ToAccountPeriod()
            {
                return Year.HasValue && Month.HasValue ? new DateTime(Year.Value, Month.Value, 1) : null;
            }
        }
    }
}
