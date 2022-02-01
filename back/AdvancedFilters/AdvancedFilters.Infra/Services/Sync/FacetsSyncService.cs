using AdvancedFilters.Domain.Facets;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Domain.Sync;
using AdvancedFilters.Infra.Storage.DAO;
using AdvancedFilters.Infra.Storage.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Infra.Services.Sync
{
    internal class MiaouFacetDto
    {
        public FacetScope Scope { get; }
        public string Code { get; }
        public string CultureKey { get; }
        public MiaouFacetResultDto Result { get; }

        public int EnvironmentId { get; set; }
        public string ApplicationId { get; set; }
    }

    internal class MiaouFacetResultDto
    {
        public FacetType Type { get; }
    }

    public class FacetsSyncService : IFacetsSyncService
    {
        private readonly IBulkUpsertService _bulkUpsertService;

        public FacetsSyncService(IBulkUpsertService bulkUpsertService)
        {
            _bulkUpsertService = bulkUpsertService;
        }

        public async Task SyncTenantsFacetsAsync(List<Environment> environments, SyncStrategy syncStrategy)
        {
            var allFacetDtos = new List<MiaouFacetDto>();

            foreach (var e in environments)
            {
                var envFacetDtos = new List<MiaouFacetDto>();

                foreach (var a in e.AppInstances)
                {
                    var dtos = await FetchAppInstanceFacetDtosAsync(e, a);
                    envFacetDtos.AddRange(dtos);
                }

                // try catch : log errors and continue to next app environment
                allFacetDtos.AddRange(envFacetDtos);
            }

            var facetsAndValues = GetFacets(allFacetDtos);

            var config = new BulkUpsertConfig
            {
                Filter = // TODO
            };
            await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(facetsAndValues.Facets, config);
            await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(facetsAndValues.EnvironmentDaos, config);
            await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(facetsAndValues.EstablishmentDaos, config);
        }

        public async Task PurgeEverythingAsync()
        {
            //await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(facetsAndValues.Facets, config);
            //await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(facetsAndValues.EnvironmentDaos, config);
            //await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(facetsAndValues.EstablishmentDaos, config);
        }

        public async Task PurgeTenantsAsync(List<Environment> environments)
        {

        }

        private async Task<IReadOnlyCollection<MiaouFacetDto>> FetchAppInstanceFacetDtosAsync(Environment e, AppInstance a)
        {
            var dtos = new List<MiaouFacetDto>();
            // find endpoint for corresponding app
            // GET facets
            // Polymorphic deserialize
            // add applicationId to each facet
            foreach (var dto in dtos)
            {
                dto.EnvironmentId = e.Id;
                dto.ApplicationId = a.ApplicationId;
            }
            // try catch : log errors and continue to next app instance

            return dtos;
        }

        private FacetsAndValues GetFacets(IEnumerable<MiaouFacetDto> dtos)
        {
            var facetsAndValues = new FacetsAndValues();

            foreach (var dto in dtos.Where(d => d.Scope != FacetScope.Unknown))
            {
                var identifier = new Facet
                {
                    Code = dto.Code,
                    ApplicationId = dto.ApplicationId,
                    Scope = dto.Scope,
                    Type = dto.Result.Type,
                };
                facetsAndValues.Facets.Add(identifier);

                switch (dto.Scope)
                {
                    case FacetScope.Environment:
                        facetsAndValues.EnvironmentDaos.AddRange(dto.ToEnvironmentDaos());
                        break;
                    case FacetScope.Establishment:
                        facetsAndValues.EstablishmentDaos.AddRange(dto.ToEstablishmentDaos());
                        break;
                    default:
                        break;
                }
            }

            return facetsAndValues;
        }

        private class FacetsAndValues
        {
            public HashSet<Facet> Facets { get; } = new();
            public List<EnvironmentFacetValueDao> EnvironmentDaos { get; } = new();
            public List<EstablishmentFacetValueDao> EstablishmentDaos { get; } = new();
        }
    }

    internal static class MiaouFacetDtoExtensions
    {
        public static IReadOnlyCollection<EnvironmentFacetValueDao> ToEnvironmentDaos(this MiaouFacetDto dto)
        {

        }

        public static IReadOnlyCollection<EstablishmentFacetValueDao> ToEstablishmentDaos(this MiaouFacetDto dto)
        {

        }
    }
}
