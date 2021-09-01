using AdvancedFilters.Domain.Billing.Models;
using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Domain.Instance.Models;
using System.Collections.Generic;

namespace AdvancedFilters.Infra.Services.Sync.Dtos
{
    public interface IDto<T>
        where T : class
    {
        List<T> ToItems();
    }

    internal class EnvironmentsDto : IDto<Environment>
    {
        public List<Environment> Items { get; set; }

        public List<Environment> ToItems()
        {
            return Items;
        }
    }

    internal class EstablishmentsDto : IDto<Establishment>
    {
        public List<Establishment> Items { get; set; }

        public List<Establishment> ToItems()
        {
            return Items;
        }
    }

    internal class AppInstancesDto : IDto<AppInstance>
    {
        public List<AppInstance> Items { get; set; }

        public List<AppInstance> ToItems()
        {
            return Items;
        }
    }

    internal class LegalUnitsDto : IDto<LegalUnit>
    {
        public List<LegalUnit> Items { get; set; }

        public List<LegalUnit> ToItems()
        {
            return Items;
        }
    }

    internal class ClientsDto : IDto<Client>
    {
        public List<Client> Items { get; set; }

        public List<Client> ToItems()
        {
            return Items;
        }
    }

    internal class AppContactsDto : IDto<AppContact>
    {
        public List<AppContact> Items { get; set; }

        public List<AppContact> ToItems()
        {
            return Items;
        }
    }

    internal class ClientContactsDto : IDto<ClientContact>
    {
        public List<ClientContact> Items { get; set; }

        public List<ClientContact> ToItems()
        {
            return Items;
        }
    }

    internal class SpecializedContactsDto : IDto<SpecializedContact>
    {
        public List<SpecializedContact> Items { get; set; }

        public List<SpecializedContact> ToItems()
        {
            return Items;
        }
    }
}
