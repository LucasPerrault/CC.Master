using System;
using System.Net;

namespace Shared.Infra.Remote.Exceptions
{
    public class RemoteServiceException : ApplicationException
    {
        private HttpStatusCode StatusCode { get; }
        public string OriginalMessage { get; }

        internal RemoteServiceException(string remoteAppName, HttpStatusCode statusCode, string message)
            : base($"Caught {remoteAppName} {statusCode} with message : {message}")
        {
            StatusCode = statusCode;
            OriginalMessage = message;
        }
    }
}
