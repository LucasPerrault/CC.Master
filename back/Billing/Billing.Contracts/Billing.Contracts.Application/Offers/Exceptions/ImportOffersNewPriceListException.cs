using Lucca.Core.Shared.Domain.Exceptions;
using Newtonsoft.Json;

namespace Billing.Contracts.Application.Offers.Exceptions
{
    public class ImportOffersNewPriceListException : BadRequestException
    {
        [JsonProperty(PropertyName = "errors")]

        public string Errors { get; set; }

        public ImportOffersNewPriceListException(string errors) : base("Bad new price list format")
        {
            Errors = errors;
        }
    }
}
