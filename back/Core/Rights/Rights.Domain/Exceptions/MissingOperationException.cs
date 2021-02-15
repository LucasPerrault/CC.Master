using Lucca.Core.Shared.Domain.Exceptions;

namespace Rights.Domain.Exceptions
{
    public class MissingOperationException : ForbiddenException
    {
        public MissingOperationException(Operation op)
            : base($"Operation {op} is missing")
        { }
    }
}
