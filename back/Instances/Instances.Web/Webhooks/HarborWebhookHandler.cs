using Instances.Application.Webhooks.Harbor;
using Instances.Application.Webhooks.Harbor.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Tools;

namespace Instances.Web.Webhooks
{
    public class HarborWebhookHandler
    {
        private readonly IHarborWebhookService _harborWebhookService;

        public HarborWebhookHandler(IHarborWebhookService harborWebhookService)
        {
            _harborWebhookService = harborWebhookService;
        }

        public async Task<IActionResult> HandleHarborAsync(HarborWebhookPayload payload)
        {
            if (payload == null)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            if (!Enum.IsDefined(payload.Type))
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }

            await _harborWebhookService.HandleWebhookAsync(payload);
            return new StatusCodeResult(StatusCodes.Status202Accepted);
        }
    }
}
