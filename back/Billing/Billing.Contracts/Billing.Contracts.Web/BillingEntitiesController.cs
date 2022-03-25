using Billing.Contracts.Domain.Clients;
using Lucca.Core.Api.Abstractions.Paging;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Tools;

namespace Billing.Contracts.Web;


[ApiController, Route("/api/billing-entities")]
public class BillingEntitiesController
{
    [HttpGet]
    public Page<BillingEntityDto> Get()
    {
        var entities = EnumExtensions.GetAllEnumsExcept(BillingEntity.Unknown);
        return new Page<BillingEntityDto>
        {
            Items = entities.Select
                (
                    billingEntity => new BillingEntityDto
                    {
                        Id = (int) billingEntity,
                        Code = billingEntity,
                        Name = billingEntity.GetDescription(),
                    }
                ),
            Count = entities.Length,
        };
    }

    public class BillingEntityDto
    {
        public int Id { get; set; }
        public BillingEntity Code { get; set; }
        public string Name { get; set; }
    }
}
