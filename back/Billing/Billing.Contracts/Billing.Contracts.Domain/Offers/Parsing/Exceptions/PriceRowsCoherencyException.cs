using Lucca.Core.Shared.Domain.Exceptions;

namespace Billing.Contracts.Domain.Offers.Parsing.Exceptions
{
    public class PriceRowsCoherencyException : BadRequestException
    {
        public PriceRowsCoherencyException(int lineNumber, int maxIncludedCount, int minIncludedCount)
            : base($"Line {lineNumber}, previous max '{maxIncludedCount}' is not directly before current min {minIncludedCount}")
        {

        }
    }

    public class OfferRowStartsOnException : BadRequestException
    {
        public OfferRowStartsOnException(int lineNumber)
            : base($"Line {lineNumber}, startOn must be specified")
        {

        }
    }
}
