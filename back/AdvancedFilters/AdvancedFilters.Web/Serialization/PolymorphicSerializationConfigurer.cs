using AdvancedFilters.Domain.Facets;
using AdvancedFilters.Domain.Filters.Models;
using System;
using Tools;

namespace AdvancedFilters.Web.Serialization
{
    internal static class PolymorphicSerializationExtensions
    {
        public static IPolymorphicSerializerBuilder ConfigureBase(this IPolymorphicSerializerBuilder builder)
        {
            return builder
                .WithPolymorphism<IEnvironmentFacetValue, FacetType>(nameof(IEnvironmentFacetValue.Type))
                    .AddMatch<EnvironmentFacetValue<int>>(FacetType.Integer)
                    .AddMatch<EnvironmentFacetValue<string>>(FacetType.String)
                    .AddMatch<EnvironmentFacetValue<decimal>>(FacetType.Decimal)
                    .AddMatch<EnvironmentFacetValue<decimal>>(FacetType.Percentage)
                    .AddMatch<EnvironmentFacetValue<DateTime>>(FacetType.DateTime)
                .WithPolymorphism<IEstablishmentFacetValue, FacetType>(nameof(IEstablishmentFacetValue.Type))
                    .AddMatch<EstablishmentFacetValue<int>>(FacetType.Integer)
                    .AddMatch<EstablishmentFacetValue<string>>(FacetType.String)
                    .AddMatch<EstablishmentFacetValue<decimal>>(FacetType.Decimal)
                    .AddMatch<EstablishmentFacetValue<decimal>>(FacetType.Percentage)
                    .AddMatch<EstablishmentFacetValue<DateTime>>(FacetType.DateTime);
        }

        public static IPolymorphicSerializerBuilder ConfigureCriterions<TCriterion>(this IPolymorphicSerializerBuilder builder)
            where TCriterion : AdvancedCriterion
        {
            return builder
                .WithPolymorphism<IAdvancedFilter, FilterElementTypes>(nameof(IAdvancedFilter.FilterElementType))
                    .AddMatch<TCriterion>(FilterElementTypes.Criterion)
                    .AddMatch<FilterCombination>(FilterElementTypes.LogicalOperator)
                .WithPolymorphism<IEnvironmentFacetCriterion, FacetType>(nameof(IEnvironmentFacetCriterion.Type))
                    .AddMatch<SingleFacetIntValueComparisonCriterion>(FacetType.Integer)
                    .AddMatch<SingleFacetValueComparisonCriterion<string>>(FacetType.String)
                    .AddMatch<SingleFacetDecimalValueComparisonCriterion>(FacetType.Decimal)
                    .AddMatch<SingleFacetDecimalValueComparisonCriterion>(FacetType.Percentage)
                    .AddMatch<SingleFacetDateTimeValueComparisonCriterion>(FacetType.DateTime);
        }
    }
}
