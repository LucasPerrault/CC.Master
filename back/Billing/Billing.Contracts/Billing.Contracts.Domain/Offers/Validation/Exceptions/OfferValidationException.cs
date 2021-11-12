using Lucca.Core.Shared.Domain.Exceptions;

namespace Billing.Contracts.Domain.Offers.Validation.Exceptions
{
    public class OfferValidationException : DomainException
    {
        public OfferValidationException(string message)
            : base(DomainExceptionCode.BadRequest, message)
        { }
    }
}
