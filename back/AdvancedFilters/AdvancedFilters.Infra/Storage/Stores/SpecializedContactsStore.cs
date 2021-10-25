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

        public Task<Page<string>> GetAllRoleCodesAsync()
        {
            var roleCodes = _dbContext
                .Set<SpecializedContact>()
                .AsNoTracking()
                .Select(c => c.RoleCode)
                .Distinct()
                .ToList();

            return Task.FromResult(new Page<string>
            {
                Count = roleCodes.Count,
                Items = roleCodes
            });
        }

        public Task<Page<SpecializedContact>> SearchAsync(IPageToken pageToken, IAdvancedFilter filter)
        {
            var contacts = SpecializedContacts.Filter(filter);
            return _queryPager.ToPageAsync(contacts, pageToken);
        }

        private IQueryable<SpecializedContact> Get(SpecializedContactFilter filter)
        {
            return SpecializedContacts
                .WhereMatches(filter)
                .AsNoTracking();
        }

        private IQueryable<SpecializedContact> SpecializedContacts => _dbContext
            .Set<SpecializedContact>()
            .Include(c => c.Environment).ThenInclude(e => e.AppInstances)
            .Include(c => c.Establishment).ThenInclude(e => e.LegalUnit).ThenInclude(lu => lu.Country);
    }

    internal static class SpecializedContactQueryableExtensions
    {
        public static IQueryable<SpecializedContact> WhereMatches(this IQueryable<SpecializedContact> contacts, SpecializedContactFilter filter)
        {
            return contacts
                .WhenNotNullOrEmpty(filter.RoleCodes).ApplyWhere(c => filter.RoleCodes.Contains(c.RoleCode))
                .WhenNotNullOrEmpty(filter.EnvironmentIds).ApplyWhere(c => filter.EnvironmentIds.Contains(c.EnvironmentId))
                .Apply(filter.IsActive).To(c => !c.ExpiresAt.HasValue || c.ExpiresAt > DateTime.Now)
                .Apply(filter.IsConfirmed).To(c => c.IsConfirmed);
        }
    }
}
