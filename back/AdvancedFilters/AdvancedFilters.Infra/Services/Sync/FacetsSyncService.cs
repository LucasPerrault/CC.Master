using AdvancedFilters.Domain.Facets;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Domain.Sync;
using AdvancedFilters.Infra.Services.Sync.Dtos.Facets;
using AdvancedFilters.Infra.Storage.DAO;
using AdvancedFilters.Infra.Storage.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Infra.Services.Sync
{
    public class FacetsSyncService : IFacetsSyncService
    {
        private readonly IBulkUpsertService _bulkUpsertService;
        private readonly IFacetsStore _store;

        public FacetsSyncService(IBulkUpsertService bulkUpsertService, IFacetsStore store)
        {
            _bulkUpsertService = bulkUpsertService;
            _store = store;
        }

        public async Task SyncTenantsFacetsAsync(List<Environment> environments, SyncStrategy syncStrategy)
        {
            if (syncStrategy == SyncStrategy.SyncEverything)
            {
                await PurgeEverythingAsync();
            }

            var allFacetDtos = new List<FacetDtoWithContext>();

            foreach (var e in environments)
            {
                foreach (var a in e.AppInstances)
                {
                    var dtos = await FetchAppInstanceFacetDtosAsync(e, a);
                    allFacetDtos.AddRange(dtos);
                }

                // try catch : log errors and continue to next app environment
            }

            var facetsAndValues = await BuildFacetsAndValuesAsync(allFacetDtos);

            var environmentIds = environments.Select(e => e.Id).ToHashSet();
            var envUpsertConfig = GetValuesUpsertConfig<EnvironmentFacetValueDao>(syncStrategy, environmentIds);
            envUpsertConfig.IncludeSubEntities = true;
            var etsUpsertConfig = GetValuesUpsertConfig<EstablishmentFacetValueDao>(syncStrategy, environmentIds);
            etsUpsertConfig.IncludeSubEntities = true;

            await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(facetsAndValues.EnvironmentDaos, envUpsertConfig);
            await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(facetsAndValues.EstablishmentDaos, etsUpsertConfig);
        }

        public async Task PurgeEverythingAsync()
        {
            var config = GetUpsertEverythingConfig();
            await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(new List<Facet>(), config);
            await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(new List<EnvironmentFacetValueDao>(), config);
            await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(new List<EstablishmentFacetValueDao>(), config);
        }

        public async Task PurgeTenantsAsync(List<Environment> environments)
        {
            var syncStrategy = SyncStrategy.SyncSpecificEnvironmentsOnly;

            var environmentIds = environments.Select(e => e.Id).ToHashSet();
            var envUpsertConfig = GetValuesUpsertConfig<EnvironmentFacetValueDao>(syncStrategy, environmentIds);
            var etsUpsertConfig = GetValuesUpsertConfig<EstablishmentFacetValueDao>(syncStrategy, environmentIds);

            await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(new List<Facet>(), GetUpsertEverythingConfig());
            await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(new List<EnvironmentFacetValueDao>(), envUpsertConfig);
            await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(new List<EstablishmentFacetValueDao>(), etsUpsertConfig);
        }

        private async Task<IReadOnlyCollection<FacetDtoWithContext>> FetchAppInstanceFacetDtosAsync(Environment e, AppInstance a)
        {
            var dtos = new List<IFacetDto>();
            // find endpoint for corresponding app
            // GET facets
            // Polymorphic deserialize

            // try catch : log errors and continue to next app instance

            dtos.AddRange(Enumerable.Range(1, 20).Select(i =>
                new EnvFacetDto
                {
                    Code = $"miaou-env-{i%3}",
                    CultureKey = "en",
                    Scope = FacetScope.Environment,
                    Result = new EnvironmentSingleResultDto<int> { Type = FacetType.Integer, Value = i }
                }
            ));

            return dtos
                .Select(dto => new FacetDtoWithContext(dto, e.Id, a.ApplicationId))
                .ToList();
        }

        private async Task<FacetsAndValues> BuildFacetsAndValuesAsync(IEnumerable<FacetDtoWithContext> dtos)
        {
            var validDtos = dtos.Where(d => d.FacetDto.Scope != FacetScope.Unknown);
            //var dtoIdentifiers = validDtos
            //    .Select(d =>
            //        new FacetIdentifier
            //        {
            //            ApplicationId = d.Context.ApplicationId,
            //            Code = d.FacetDto.Code
            //        }
            //    )
            //    .ToHashSet();
            //var existingFacets = (await _store.GetByIdentifiersAsync(dtoIdentifiers)).ToHashSet();

            var facetsAndValues = new FacetsAndValues();
            foreach (var dto in validDtos)
            {
                var facet = new Facet
                {
                    Code = dto.FacetDto.Code,
                    ApplicationId = dto.Context.ApplicationId,
                    Scope = dto.FacetDto.Scope,
                    Type = dto.FacetDto.Type,
                };
                //if (!existingFacets.Contains(facet))
                //{
                //    facetsAndValues.Facets.Add(facet);
                //}

                switch (dto.FacetDto.Scope)
                {
                    case FacetScope.Environment:
                        facetsAndValues.EnvironmentDaos.AddRange(dto.FacetDto.ToEnvironmentDaos(dto.Context.EnvironmentId, facet));
                        break;
                    case FacetScope.Establishment:
                        facetsAndValues.EstablishmentDaos.AddRange(dto.FacetDto.ToEstablishmentDaos(dto.Context.EnvironmentId, facet));
                        break;
                    default:
                        // TODO
                        break;
                }
            }

            return facetsAndValues;
        }

        private BulkUpsertConfig GetUpsertEverythingConfig()
            => new BulkUpsertConfig { Filter = UpsertFilter.Everything() };
        private BulkUpsertConfig GetValuesUpsertConfig<T>(SyncStrategy syncStrategy, HashSet<int> envIds)
            where T : class, IFacetValueDao
            => new BulkUpsertConfig { Filter = UpsertFilter.ForStrategy<T>(syncStrategy, envIds, v => v.EnvironmentId) };

        private class FacetsAndValues
        {
            //public HashSet<Facet> Facets { get; } = new();
            public List<EnvironmentFacetValueDao> EnvironmentDaos { get; } = new();
            public List<EstablishmentFacetValueDao> EstablishmentDaos { get; } = new();
        }

        private class FacetDtoWithContext
        {
            public IFacetDto FacetDto { get; }
            public FacetContext Context { get; }

            public FacetDtoWithContext(IFacetDto facetDto, int envId, string appId)
            {
                FacetDto = facetDto;
                Context = new FacetContext(envId, appId);
            }
        }

        internal class FacetContext
        {
            public int EnvironmentId { get; }
            public string ApplicationId { get; }

            public FacetContext(int envId, string appId)
            {
                EnvironmentId = envId;
                ApplicationId = appId;
            }
        }
    }
}
