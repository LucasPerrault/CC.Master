using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Rights.Domain;
using Rights.Domain.Abstractions;
using System.Threading.Tasks;

namespace Rights.Web.Attributes
{

    public abstract class OperationAttribute : ActionFilterAttribute
    {
        private readonly Operation[] _operations;

        public OperationAttribute(Operation[] operations)
        {
            _operations = operations;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var rightsService = context.HttpContext.RequestServices.GetService<IRightsService>();
            await ThrowIfInvalidRightsAsync(rightsService, _operations);
            await next();
        }

        protected abstract Task ThrowIfInvalidRightsAsync(IRightsService rightsService, Operation[] operations);
    }

    public class ForbidIfMissingAttribute : OperationAttribute
    {
        public ForbidIfMissingAttribute(params Operation[] operations) : base(operations)
        { }

        protected override async Task ThrowIfInvalidRightsAsync(IRightsService rightsService, Operation[] operations)
        {
            await rightsService.ThrowIfAllOperationsAreMissingAsync(operations);
        }
    }
}
