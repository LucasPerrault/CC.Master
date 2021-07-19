using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Lock.Web
{
    public class OneRequestAtATimeAttribute: ActionFilterAttribute
    {

        public string LockName { get; }
        public TimeSpan Timeout { get; }

        public OneRequestAtATimeAttribute(string lockName, double timeoutInSeconds)
        {
            LockName = lockName;

            // attribute won't take timespan as an argument
            Timeout = TimeSpan.FromSeconds(timeoutInSeconds);
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var lockService = context.HttpContext.RequestServices.GetService<ILockService>();
            using var acquiredLock = await lockService.TakeLockAsync(LockName, Timeout);
            await next();
        }
    }
}
