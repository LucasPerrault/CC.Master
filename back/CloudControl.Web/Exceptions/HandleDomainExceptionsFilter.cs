using Lucca.Core.Shared.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;
using System.Threading.Tasks;

namespace CloudControl.Web.Exceptions
{
    public class HandleDomainExceptionsFilter : IExceptionFilter,IAsyncExceptionFilter
    {
        public HandleDomainExceptionsFilter()
        {
        }

        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case DomainException de:
                    context.Result = new StatusCodeResult((int)de.Status);
                    break;
            }
        }

        public Task OnExceptionAsync(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case DomainException de:
                    context.Result = new StatusCodeResult((int)de.Status);
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
