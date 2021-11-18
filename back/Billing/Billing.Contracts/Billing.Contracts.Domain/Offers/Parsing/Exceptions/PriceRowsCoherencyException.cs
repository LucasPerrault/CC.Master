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
}
