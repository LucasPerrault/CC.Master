using Lucca.Core.Shared.Domain.Exceptions;
using Newtonsoft.Json;

namespace Billing.Contracts.Application.Offers.Exceptions
{
    public class ImportOffersNewOfferRowException : BadRequestException
    {
        [JsonProperty(PropertyName = "errors")]

        public string Errors { get; set; }

        public ImportOffersNewOfferRowException(string errors) : base("Bad offer row format")
        {
            Errors = errors;
        }
    }
}
