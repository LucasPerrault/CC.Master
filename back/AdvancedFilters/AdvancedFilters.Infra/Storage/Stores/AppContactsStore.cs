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
    public class AppContactsStore : IAppContactsStore
    {
        private readonly AdvancedFiltersDbContext _dbContext;
        private readonly IQueryPager _queryPager;

        public AppContactsStore(AdvancedFiltersDbContext dbContext, IQueryPager queryPager)
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
        }

        public Task<Page<AppContact>> GetAsync(IPageToken pageToken, AppContactFilter filter)
        {
            var contacts = Get(filter);
            return _queryPager.ToPageAsync(contacts, pageToken);
        }

        public Task<Page<AppContact>> SearchAsync(IPageToken pageToken, IAdvancedFilter filter)
        {
            var contacts = AppContacts.Filter(filter).AsNoTracking();
            return _queryPager.ToPageAsync(contacts, pageToken);
        }

        public Task<List<AppContact>> SearchAsync(IAdvancedFilter filter)
        {
            var contacts = AppContacts.Filter(filter);
            return contacts.AsNoTracking().ToListAsync();
        }

        private IQueryable<AppContact> Get(AppContactFilter filter)
        {
            return AppContacts
                .WhereMatches(filter)
                .AsNoTracking();
        }

        private IQueryable<AppContact> AppContacts => _dbContext
            .Set<AppContact>()
            .Include(c => c.Environment).ThenInclude(c => c.Accesses)
            .Include(c => c.Establishment).ThenInclude(e => e.LegalUnit).ThenInclude(lu => lu.Country)
            .Include(c => c.AppInstance);
    }

    internal static class AppContactQueryableExtensions
    {
        public static IQueryable<AppContact> WhereMatches(this IQueryable<AppContact> contacts, AppContactFilter filter)
        {
            return contacts
                .WhenNotNullOrEmpty(filter.ApplicationIds).ApplyWhere(c => filter.ApplicationIds.Contains(c.AppInstance.ApplicationId))
                .WhenNotNullOrEmpty(filter.EnvironmentIds).ApplyWhere(c => filter.EnvironmentIds.Contains(c.AppInstance.EnvironmentId))
                .Apply(filter.IsActive).To(c => !c.ExpiresAt.HasValue || c.ExpiresAt > DateTime.Now)
                .Apply(filter.IsConfirmed).To(c => c.IsConfirmed);
        }
    }
}
