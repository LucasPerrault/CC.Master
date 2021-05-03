using Lucca.Core.Shared.Domain.Exceptions;
using Lucca.Logs.AspnetCore;
using System;
using System.Net;

namespace CloudControl.Web.Exceptions
{
    public class CloudControlExceptionsQualifier : GenericExceptionQualifier
    {
        public override bool LogToOpserver(Exception exception) => !(exception is DomainException);

        public override bool DisplayExceptionDetails(Exception exception) => !(exception is DomainException);

        public override HttpStatusCode? StatusCode(Exception exception)
        {
            if (exception is DomainException domainException)
            {
                return (HttpStatusCode)domainException.Status;
            }

            return base.StatusCode(exception);
        }

        public override string PreferedResponseType(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            return "application/json";
        }
    }
}
