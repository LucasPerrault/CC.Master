using AdvancedFilters.Domain.Billing.Models;
using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Domain.Instance.Models;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedFilters.Infra.Services.Sync.Dtos
{
    public interface IDto<T>
        where T : class
    {
        List<T> ToItems();
    }

    public class EnvironmentsDto : IDto<Environment>
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

    public class AppInstancesDto : ApiV3Dto<AppInstance>, IDto<AppInstance>
    {
        public List<AppInstance> ToItems()
        {
            return Data.Items;
        }
    }

    public class LegalUnitsDto : IDto<LegalUnit>
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



    internal class ContactDtoUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mail { get; set; }
        public int EstablishmentId { get; set; }
    }

    internal class ContactDtoRole
    {
        public string Code { get; set; }
    }

    internal class AppContactsDto : IDto<AppContact>
    {
        public class AppContactDto : AppContact
        {
            public ContactDtoUser User { get; set; }
        }

        public List<AppContactDto> Items { get; set; }

        AppContact ToAppContact(AppContactDto c)
        {
            c.UserFirstName = c.User.FirstName;
            c.UserLastName = c.User.LastName;
            c.UserMail = c.User.Mail;
            c.EstablishmentId = c.User.EstablishmentId;
            return c;
        }

        public List<AppContact> ToItems() => Items.Select(ToAppContact).ToList();

    }

    internal class ClientContactsDto : IDto<ClientContact>
    {
        public class ClientContactDto : ClientContactCore
        {
            public ContactDtoUser User { get; set; }
            public ContactDtoRole Role { get; set; }
        }

        public List<ClientContactDto> Items { get; set; }

        ClientContact ToClientContact(ClientContactDto c)
        {
            c.UserFirstName = c.User.FirstName;
            c.UserLastName = c.User.LastName;
            c.UserMail = c.User.Mail;
            c.EstablishmentId = c.User.EstablishmentId;
            return new ClientContact
            {
                Id = c.Id,
                ClientId = c.ClientId,
                EnvironmentId = c.EnvironmentId,
                RoleCode = c.Role.Code,
                CreatedAt = c.CreatedAt,
                ExpiresAt = c.ExpiresAt,
                EstablishmentId = c.EstablishmentId,
                IsConfirmed = c.IsConfirmed,
                RoleId = c.RoleId,
                UserId = c.UserId
            };
        }

        public List<ClientContact> ToItems() => Items.Select(ToClientContact).ToList();
    }

    internal class SpecializedContactsDto : IDto<SpecializedContact>
    {
        public class SpecializedContactDto : SpecializedContact
        {
            public ContactDtoUser User { get; set; }
            public ContactDtoRole Role { get; set; }
        }

        public List<SpecializedContactDto> Items { get; set; }

        SpecializedContact ToSpecializedContact(SpecializedContactDto c)
        {
            c.UserFirstName = c.User.FirstName;
            c.UserLastName = c.User.LastName;
            c.UserMail = c.User.Mail;
            c.EstablishmentId = c.User.EstablishmentId;
            c.RoleCode = c.Role.Code;
            return c;
        }

        public List<SpecializedContact> ToItems() => Items.Select(ToSpecializedContact).ToList();
    }
}
