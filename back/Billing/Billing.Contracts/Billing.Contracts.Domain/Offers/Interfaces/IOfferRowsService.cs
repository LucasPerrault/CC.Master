using Billing.Contracts.Domain.Offers.Parsing;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Billing.Contracts.Domain.Offers.Interfaces
{
    public interface IOfferRowsService
    {
        Task<List<ParsedOffer>> UploadAsync(Stream stream);
    }
}
