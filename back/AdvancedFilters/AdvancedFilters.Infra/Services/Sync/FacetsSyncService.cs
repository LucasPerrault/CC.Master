using System;
using AdvancedFilters.Domain.Facets;
using AdvancedFilters.Domain.Sync;
using AdvancedFilters.Infra.Services.Sync.Dtos.Facets;
using AdvancedFilters.Infra.Storage.DAO;
using AdvancedFilters.Infra.Storage.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Infra.Services.Sync
{
    public class FacetsSyncService : IFacetsSyncService
    {
        private readonly IBulkUpsertService _bulkUpsertService;
        private readonly HttpClient _httpClient;
        private readonly IFacetsStore _facetsStore;

        public FacetsSyncService(IBulkUpsertService bulkUpsertService, HttpClient httpClient, IFacetsStore facetsStore)
        {
            _bulkUpsertService = bulkUpsertService;
            _httpClient = httpClient;
            _facetsStore = facetsStore;
        }

        public async Task SyncTenantsFacetsAsync(List<Environment> environments, SyncStrategy syncStrategy)
        {
            if (syncStrategy == SyncStrategy.SyncEverything)
            {
                await PurgeEverythingAsync();
            }

            var nonExistingFacets = new List<Facet>();
            var facets = await _facetsStore.GetAsync(FacetFilter.All());
            var facetValueBuilder = new FacetValuesBuilder();

            foreach (var environment in environments)
            {
                await SyncTenantFacetsAsync(environment, facets, nonExistingFacets, facetValueBuilder);
            }

            await _facetsStore.CreateManyAsync(nonExistingFacets);

            var environmentIds = environments.Select(e => e.Id).ToHashSet();
            var envUpsertConfig = GetValuesUpsertConfig<EnvironmentFacetValueDao>(syncStrategy, environmentIds);
            var etsUpsertConfig = GetValuesUpsertConfig<EstablishmentFacetValueDao>(syncStrategy, environmentIds);
            await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(facetValueBuilder.EnvironmentDaos, envUpsertConfig);
            await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(facetValueBuilder.EstablishmentDaos, etsUpsertConfig);
        }

        public async Task PurgeEverythingAsync()
        {
            var config = GetUpsertEverythingConfig();
            await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(new List<EnvironmentFacetValueDao>(), config);
            await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(new List<EstablishmentFacetValueDao>(), config);
        }

        public async Task PurgeTenantsAsync(List<Environment> environments)
        {
            var syncStrategy = SyncStrategy.SyncSpecificEnvironmentsOnly;

            var environmentIds = environments.Select(e => e.Id).ToHashSet();
            var envUpsertConfig = GetValuesUpsertConfig<EnvironmentFacetValueDao>(syncStrategy, environmentIds);
            var etsUpsertConfig = GetValuesUpsertConfig<EstablishmentFacetValueDao>(syncStrategy, environmentIds);

            await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(new List<EnvironmentFacetValueDao>(), envUpsertConfig);
            await _bulkUpsertService.InsertOrUpdateOrDeleteAsync(new List<EstablishmentFacetValueDao>(), etsUpsertConfig);
        }

        public async Task SyncTenantFacetsAsync(Environment environment, List<Facet> facets, List<Facet> nonExistingFacets, FacetValuesBuilder builder)
        {
            foreach (var dto in await FetchFacetDtosAsync(environment))
            {
                var facet = facets.SingleOrDefault(f => f.ApplicationId == dto.Module && f.Code == dto.Code);
                if (facet is null)
                {
                    facet = new Facet
                    {
                        Code = dto.Code,
                        ApplicationId = dto.Module,
                        Scope = dto.Scope,
                        Type = dto.Type
                    };
                    nonExistingFacets.Add(facet);
                }

                switch (dto.Scope)
                {
                    case FacetScope.Environment:
                        builder.EnvironmentDaos.AddRange(dto.ToEnvironmentDaos(environment.Id, facet));
                        break;
                    case FacetScope.Establishment:
                        builder.EstablishmentDaos.AddRange(dto.ToEstablishmentDaos(environment.Id, facet));
                        break;
                    default:
                        // TODO : log error
                        break;
                }
            }
        }


        private async Task BuildFacetValuesAsync(List<Environment> environments, List<Facet> nonExistingFacets, FacetValuesBuilder builder)
        {
            var existingFacets = await _facetsStore.GetAsync(FacetFilter.All());

            foreach (var environment in environments)
            {
                var dtos = await FetchFacetDtosAsync(environment);

                foreach (var dto in dtos)
                {
                    var facet = existingFacets.SingleOrDefault(f => f.ApplicationId == dto.Module && f.Code == dto.Code);
                    if (facet is null)
                    {
                        facet = new Facet
                        {
                            Code = dto.Code,
                            ApplicationId = dto.Module,
                            Scope = dto.Scope,
                            Type = dto.Type
                        };
                        nonExistingFacets.Add(facet);
                    }

                    switch (dto.Scope)
                    {
                        case FacetScope.Environment:
                            builder.EnvironmentDaos.AddRange(dto.ToEnvironmentDaos(environment.Id, facet));
                            break;
                        case FacetScope.Establishment:
                            builder.EstablishmentDaos.AddRange(dto.ToEstablishmentDaos(environment.Id, facet));
                            break;
                        default:
                            // TODO : log error
                            break;
                    }
                }

            }
        }

        private async Task<List<IFacetDto>> FetchFacetDtosAsync(Environment environment)
        {
            /*var clientUri = $"{ environment.Subdomain }.ilucca.local";
            var uri = new Uri($"{ clientUri }/back-office/facets");
            var response = await _httpClient.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                // TODO : log error
                return new List<IFacetDto>();
            }

            return await response.Content.ReadFromJsonAsync<List<IFacetDto>>();*/

            return new List<IFacetDto>
            {
                new EnvFacetDto
                {
                    Code = "nb-miaou",
                    Module = "WPAGGA",
                    Scope = FacetScope.Environment,
                    CultureKey = "fr",
                    Result = new EnvironmentSingleResultDto<int>
                    {
                        Type = FacetType.Integer,
                        Value = 2
                    }
                },
                new EnvFacetDto
                {
                    Code = "date-miaou",
                    Module = "WTIMMI",
                    Scope = FacetScope.Environment,
                    CultureKey = "fr",
                    Result = new EnvironmentSingleResultDto<DateTime>
                    {
                        Type = FacetType.DateTime,
                        Value = new DateTime(2022, 12, 01)
                    }
                },
                new EnvFacetDto
                {
                    Code = "non-existing-text-miaou",
                    Module = "WEXPENSES",
                    Scope = FacetScope.Environment,
                    CultureKey = "fr",
                    Result = new EnvironmentSingleResultDto<string>
                    {
                        Type = FacetType.String,
                        Value = "beeeh beeeh"
                    }
                },
            };
        }

        private BulkUpsertConfig GetUpsertEverythingConfig()
            => new BulkUpsertConfig { Filter = UpsertFilter.Everything() };
        private BulkUpsertConfig GetValuesUpsertConfig<T>(SyncStrategy syncStrategy, HashSet<int> envIds)
            where T : class, IFacetValueDao
            => new BulkUpsertConfig { Filter = UpsertFilter.ForStrategy<T>(syncStrategy, envIds, v => v.EnvironmentId) };

        private class FacetBuilder
        {
            public Environment Environment { get; set; }
            public List<Facet> Facets { get; set; }
            public List<IFacetDto> FacetDtos { get; set; }
        }

        public class FacetValuesBuilder
        {
            public List<EnvironmentFacetValueDao> EnvironmentDaos { get; } = new();
            public List<EstablishmentFacetValueDao> EstablishmentDaos { get; } = new();
        }
    }
}
