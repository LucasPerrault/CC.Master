using Lucca.Core.Shared.Domain.Exceptions;
using Newtonsoft.Json;

namespace Billing.Contracts.Application.Offers.Exceptions
{
    public class ImportOffersHeaderException : BadRequestException
    {
        [JsonProperty(PropertyName = "errors")]

        public string Errors { get; set; }

        public ImportOffersHeaderException(string errors) : base("Bad headers Format")
        {
            Errors = errors;
        }
    }
}
