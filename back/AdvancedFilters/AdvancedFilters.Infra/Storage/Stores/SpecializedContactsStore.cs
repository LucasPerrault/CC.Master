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
    public class SpecializedContactsStore : ISpecializedContactsStore
    {
        private readonly AdvancedFiltersDbContext _dbContext;
        private readonly IQueryPager _queryPager;

        public SpecializedContactsStore(AdvancedFiltersDbContext dbContext, IQueryPager queryPager)
        {
            _dbContext = dbContext;
            _queryPager = queryPager;
        }

        public Task<Page<SpecializedContact>> GetAsync(IPageToken pageToken, SpecializedContactFilter filter)
        {
            var contacts = Get(filter);
            return _queryPager.ToPageAsync(contacts, pageToken);
        }

        private IQueryable<SpecializedContact> Get(SpecializedContactFilter filter)
        {
            return SpecializedContacts
                .WhereMatches(filter);
        }

        private IQueryable<SpecializedContact> SpecializedContacts => _dbContext.Set<SpecializedContact>();
    }

    internal static class SpecializedContactQueryableExtensions
    {
        public static IQueryable<SpecializedContact> WhereMatches(this IQueryable<SpecializedContact> contacts, SpecializedContactFilter filter)
        {
            return contacts
                .When(filter.RoleId.HasValue).ApplyWhere(c => c.RoleId == filter.RoleId.Value)
                .When(filter.EnvironmentId.HasValue).ApplyWhere(c => c.EnvironmentId == filter.EnvironmentId.Value)
                .Apply(filter.IsActive).To(c => !c.ExpiresAt.HasValue || c.ExpiresAt > DateTime.Now)
                .Apply(filter.IsConfirmed).To(c => c.IsConfirmed);
        }
    }
}
