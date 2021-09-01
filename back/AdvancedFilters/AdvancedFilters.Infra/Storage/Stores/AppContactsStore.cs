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

        private IQueryable<AppContact> Get(AppContactFilter filter)
        {
            return AppContacts
                .WhereMatches(filter);
        }

        private IQueryable<AppContact> AppContacts => _dbContext.Set<AppContact>();
    }

    internal static class AppContactQueryableExtensions
    {
        public static IQueryable<AppContact> WhereMatches(this IQueryable<AppContact> contacts, AppContactFilter filter)
        {
            return contacts
                .Apply(filter.ApplicationId).To(c => c.AppInstance.ApplicationId)
                .When(filter.EnvironmentId.HasValue).ApplyWhere(c => c.AppInstance.EnvironmentId == filter.EnvironmentId.Value)
                .Apply(filter.IsActive).To(c => !c.ExpiresAt.HasValue || c.ExpiresAt > DateTime.Now)
                .Apply(filter.IsConfirmed).To(c => c.IsConfirmed);
        }
    }
}
