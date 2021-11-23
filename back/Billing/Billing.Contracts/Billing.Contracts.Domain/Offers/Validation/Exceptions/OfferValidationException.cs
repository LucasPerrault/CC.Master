using Lucca.Core.Shared.Domain.Exceptions;

namespace Billing.Contracts.Domain.Offers.Validation.Exceptions
{
    public class OfferValidationException : BadRequestException
    {
        public OfferValidationException(string message)
            : base(message)
        { }
    }
}
