using AdvancedFilters.Domain.Contacts.Filters;
using AdvancedFilters.Domain.Contacts.Interfaces;
using AdvancedFilters.Domain.Contacts.Models;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Storage.Infra.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Infra.Storage.Stores
{
    public class ClientContactsStore : IClientContactsStore
    {
        private readonly AdvancedFiltersDbContext _dbContext;
        private readonly IQueryPager _queryPager;

        public ClientContactsStore(AdvancedFiltersDbContext dbContext, IQueryPager queryPager)
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
        }

        public Task<Page<ClientContact>> GetAsync(IPageToken pageToken, ClientContactFilter filter)
        {
            var contacts = Get(filter);
            return _queryPager.ToPageAsync(contacts, pageToken);
        }

        private IQueryable<ClientContact> Get(ClientContactFilter filter)
        {
            return ClientContacts
                .WhereMatches(filter);
        }

        private IQueryable<ClientContact> ClientContacts => _dbContext.Set<ClientContact>();
    }

    internal static class ClientContactQueryableExtensions
    {
        public static IQueryable<ClientContact> WhereMatches(this IQueryable<ClientContact> contacts, ClientContactFilter filter)
        {
            return contacts
                .When(filter.RoleId.HasValue).ApplyWhere(c => c.RoleId == filter.RoleId.Value)
                .Apply(filter.ClientId).To(c => c.ClientId)
                //.When(filter.EnvironmentId.HasValue).ApplyWhere(c => c.EnvironmentId == filter.EnvironmentId.Value)
                //.When(filter.EstablishmentId.HasValue).ApplyWhere(c => c.EstablishmentId == filter.EstablishmentId.Value)
                .Apply(filter.IsActive).To(c => !c.ExpiresAt.HasValue || c.ExpiresAt > DateTime.Now) // TODO CompareDateTime ?
                .Apply(filter.IsConfirmed).To(c => c.IsConfirmed);

        }
    }
}
