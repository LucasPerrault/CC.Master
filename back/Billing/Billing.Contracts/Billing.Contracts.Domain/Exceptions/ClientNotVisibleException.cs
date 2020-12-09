using Lucca.Core.Shared.Domain.Exceptions;

namespace Billing.Contracts.Domain.Exceptions
{
    public class ClientNotVisibleException : NotFoundException
    {
        public ClientNotVisibleException()
            : base()
        { }
    }
}
