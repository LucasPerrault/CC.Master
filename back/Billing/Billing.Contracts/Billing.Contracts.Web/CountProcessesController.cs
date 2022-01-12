using Billing.Contracts.Application;
using Billing.Contracts.Domain.Common;
using Billing.Contracts.Domain.Counts;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billing.Contracts.Web
{
    [ApiController, Route("/api/count-processes")]
    public class CountProcessesController
    {
        private readonly CountProcessService _countProcessService;

        public CountProcessesController(CountProcessService countProcessService)
        {
            _countProcessService = countProcessService;
        }

        [HttpPost("launch")]
        public Task<IEnumerable<Count>> LaunchAsync([FromBody]CountProcessQuery query)
        {
            return _countProcessService.RunAsync(query.Period);
        }

        public class CountProcessQuery
        {
            public AccountingPeriod Period { get; set; }
        }
    }
}
