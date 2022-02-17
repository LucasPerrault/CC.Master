using AdvancedFilters.Domain.Facets;
using AdvancedFilters.Infra.Storage.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using Tools;

namespace AdvancedFilters.Infra.Services.Sync.Dtos.Facets
{
    public static class FacetDtoExtensions
    {
        public static readonly IPolymorphicSerializer FacetDtoSerializer = Serializer
            .WithPolymorphism<IFacetDto, FacetScope>(nameof(IFacetDto.Scope))
                .AddMatch<EnvFacetDto>(FacetScope.Environment)
                .AddMatch<EtsFacetDto>(FacetScope.Establishment)
            .WithPolymorphism<IEnvFacetResultDto, FacetType>(nameof(IEnvFacetResultDto.Type))
                .AddMatch<EnvironmentSingleResultDto<int>>(FacetType.Integer)
                .AddMatch<EnvironmentSingleResultDto<decimal>>(FacetType.Decimal)
                .AddMatch<EnvironmentSingleResultDto<decimal>>(FacetType.Percentage)
                .AddMatch<EnvironmentSingleResultDto<DateTime>>(FacetType.DateTime)
                .AddMatch<EnvironmentMultipleResultDto<string>>(FacetType.String)
            .WithPolymorphism<IEtsFacetResultDto, FacetType>(nameof(IEtsFacetResultDto.Type))
                .AddMatch<EstablishmentFacetResultDto<EstablishmentSingleValue<int>>>(FacetType.Integer)
                .AddMatch<EstablishmentFacetResultDto<EstablishmentSingleValue<decimal>>>(FacetType.Decimal)
                .AddMatch<EstablishmentFacetResultDto<EstablishmentSingleValue<decimal>>>(FacetType.Percentage)
                .AddMatch<EstablishmentFacetResultDto<EstablishmentSingleValue<DateTime>>>(FacetType.DateTime)
                .AddMatch<EstablishmentFacetResultDto<EstablishmentMultipleValue<string>>>(FacetType.String)
            .Build();

        internal static IReadOnlyCollection<EnvironmentFacetValueDao> ToEnvironmentDaos(this IFacetDto dto, int envId, Facet facet)
        {
            var envDto = dto as EnvFacetDto;

            return envDto.Result.DaoCreators
                .Select(f => f.Create(envId, facet))
                .ToList();
        }

        internal static IReadOnlyCollection<EstablishmentFacetValueDao> ToEstablishmentDaos(this IFacetDto dto, int envId, Facet facet)
        {
            var etsDto = dto as EtsFacetDto;

            return etsDto.Result.DaoCreators
                .Select(f => f.Create(envId, facet))
                .ToList();
        }
    }
}
