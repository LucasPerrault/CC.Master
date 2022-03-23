using Billing.Cmrr.Domain.Situation;
using Resources.Translations;
using System.Collections.Generic;
using System.Linq;

namespace Billing.Cmrr.Infra.Services.Export.Clients
{
    public class CmrrClientRow
    {
        public int ClientId { get; set; }
        public string Client { get; set; }
        public int ContractId { get; set; }
        public string Product { get; set; }
        public decimal UserCount { get; set; }
        public decimal Amount { get; set; }
    }

    public static class CmrrClientRowExtensions
    {
        public static IReadOnlyCollection<CmrrClientRow> ToRows(this IReadOnlyCollection<CmrrClient> clients, ICmrrTranslations translations)
        {
            return clients.Select(ToRow).ToList();
        }

        public static CmrrClientRow ToRow(this CmrrClient client)
            => new CmrrClientRow
            {
                ClientId = client.ClientId,
                Client = client.Client,
                ContractId = client.ContractId,
                Product = client.Product,
                UserCount = client.UserCount,
                Amount = client.Amount,
            };
    }
}
