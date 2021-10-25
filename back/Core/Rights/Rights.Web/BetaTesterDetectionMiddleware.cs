using Authentication.Domain;
using Microsoft.AspNetCore.Http;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System.Threading.Tasks;

namespace Rights.Web
{
    public class BetaTesterDetectionMiddleware
    {
        private readonly RequestDelegate _next;

        public BetaTesterDetectionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {

            if (context.User is CloudControlUserClaimsPrincipal)
            {
                var rightsService = (IRightsService)context.RequestServices.GetService(typeof(IRightsService));
                BetaTesterHelper.SetBetaTester(context, await rightsService.HasOperationAsync(Operation.AccessBetaFeatures));
            }

            await _next.Invoke(context);
        }
    }
}
