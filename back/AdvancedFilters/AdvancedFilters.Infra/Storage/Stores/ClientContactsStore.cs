using AdvancedFilters.Domain.Contacts.Filters;
using AdvancedFilters.Domain.Contacts.Interfaces;
using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Infra.Filters;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Api.Queryable.Paging;
using Microsoft.EntityFrameworkCore;
using Storage.Infra.Extensions;
using System;
using System.Collections.Generic;
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

        public Task<Page<ClientContact>> SearchAsync(IPageToken pageToken, IAdvancedFilter filter)
        {
            var contacts = ClientContacts.Filter(filter).AsNoTracking();
            return _queryPager.ToPageAsync(contacts, pageToken);
        }
        public Task<List<ClientContact>> SearchAsync(IAdvancedFilter filter)
        {
            var contacts = ClientContacts.Filter(filter).AsNoTracking();
            return contacts.ToListAsync();
        }

        private IQueryable<ClientContact> Get(ClientContactFilter filter)
        {
            return ClientContacts
                .WhereMatches(filter)
                .AsNoTracking();
        }

        private IQueryable<ClientContact> ClientContacts => _dbContext
            .Set<ClientContact>()
            .Include(c => c.Environment).ThenInclude(e => e.AppInstances)
            .Include(c => c.Environment).ThenInclude(e => e.Accesses).ThenInclude(a => a.Distributor)
            .Include(c => c.Establishment).ThenInclude(e => e.LegalUnit).ThenInclude(lu => lu.Country)
            .Include(c => c.Client).ThenInclude(e => e.Contracts);
    }

    internal static class ClientContactQueryableExtensions
    {
        public static IQueryable<ClientContact> WhereMatches(this IQueryable<ClientContact> contacts, ClientContactFilter filter)
        {
            return contacts
                .WhenNotNullOrEmpty(filter.RoleCodes).ApplyWhere(c => filter.RoleCodes.Contains(c.RoleCode))
                .WhenNotNullOrEmpty(filter.ClientIds).ApplyWhere(c => filter.ClientIds.Contains(c.ClientId))
                .WhenNotNullOrEmpty(filter.EnvironmentIds).ApplyWhere(c => filter.EnvironmentIds.Contains(c.EnvironmentId))
                .WhenNotNullOrEmpty(filter.EstablishmentIds).ApplyWhere(c => filter.EstablishmentIds.Contains(c.EstablishmentId))
                .Apply(filter.IsActive).To(c => !c.ExpiresAt.HasValue || c.ExpiresAt > DateTime.Now)
                .Apply(filter.IsConfirmed).To(c => c.IsConfirmed);
        }
    }
}
