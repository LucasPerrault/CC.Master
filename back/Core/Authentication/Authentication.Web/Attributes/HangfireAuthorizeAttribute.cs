using Authentication.Infra.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Authentication.Web.Attributes
{
    public class HangfireAuthorizeAttribute : ActionFilterAttribute, IAllowAnonymous
    {
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var service = context.HttpContext.RequestServices.GetService<HangfireAuthorizationService>();
            service.ThrowIfUnauthorized(context.HttpContext.Request);
            return next();
        }
    }
}
