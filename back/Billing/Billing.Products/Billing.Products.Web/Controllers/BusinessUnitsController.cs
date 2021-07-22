using Billing.Products.Domain;
using Billing.Products.Domain.Interfaces;
using Lucca.Core.Api.Abstractions.Paging;
using Microsoft.AspNetCore.Mvc;
using Rights.Domain;
using Rights.Web.Attributes;
using System.Threading.Tasks;

namespace Billing.Products.Web.Controllers
{
    [ApiController, Route("/api/business-units")]
    public class BusinessUnitsController
    {
        private readonly IBusinessUnitsStore _businessUnitsStore;

        public BusinessUnitsController(IBusinessUnitsStore businessUnitsStore)
        {
            _businessUnitsStore = businessUnitsStore;
        }

        [HttpGet, ForbidIfMissing(Operation.ReadCMRR)]
        public async Task<Page<BusinessUnit>> GetAsync()
        {
            var businessUnits = await _businessUnitsStore.GetAsync();
            return new Page<BusinessUnit>
            {
                Count = businessUnits.Count,
                Items = businessUnits
            };
        }
    }
}
