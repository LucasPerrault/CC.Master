using AdvancedFilters.Domain.Billing.Models;
using AdvancedFilters.Domain.Instance.Models;
using System.Collections.Generic;

namespace AdvancedFilters.Infra.Services
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

    internal class EstablishmentDto : IDto<Establishment>
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

    internal class LegalUnitDto : IDto<LegalUnit>
    {
        public List<LegalUnit> Items { get; set; }

        public List<LegalUnit> ToItems()
        {
            return Items;
        }
    }

    internal class ContractsDto : IDto<Contract>
    {
        public List<Contract> Items { get; set; }

        public List<Contract> ToItems()
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
}
