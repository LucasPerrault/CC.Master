using Lucca.Core.Shared.Domain.Exceptions;

namespace Rights.Domain.Exceptions
{
    public class MissingOperationsException : ForbiddenException
    {
        public MissingOperationsException(params Operation[] ops)
            : base($"Operation(s) {string.Join(", ", ops)} is(are) missing")
        { }
    }
}
