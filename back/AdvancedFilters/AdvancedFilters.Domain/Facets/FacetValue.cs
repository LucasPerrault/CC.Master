using AdvancedFilters.Domain.Filters.Builders;
using AdvancedFilters.Domain.Filters.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tools;

namespace AdvancedFilters.Domain.Facets;

public interface IFacetValue
{
    public int FacetId { get; }
    public int EnvironmentId { get; }
    public FacetIdentifier Facet { get; set; }
}

public interface IEnvironmentFacetValue : IFacetValue
{
    public int Id { get; }
    public FacetType Type { get; }
}

public class EnvironmentFacetValue<T> : IEnvironmentFacetValue
{
    public int Id { get; set; }
    public T Value { get; set; }
    public int FacetId { get; set; }
    public int EnvironmentId { get; set; }
    public FacetIdentifier Facet { get; set; }
    public FacetType Type { get; set; }
}

public class EnvironmentFacetAdvancedCriterion : AdvancedCriterion<IEnvironmentFacetValue>
{
    public FacetIdentifier Identifier { get; set; }
    public IEnvironmentFacetCriterion Value { get; set; }
    public override IQueryableExpressionBuilder<IEnvironmentFacetValue> GetExpressionBuilder(IQueryableExpressionBuilderFactory factory)
        => factory.Create(this);

}

public class EnvironmentFacetsAdvancedCriterion : EnvironmentFacetAdvancedCriterion, IListCriterion
{
    public ItemsMatching ItemsMatched { get; set; }
}

public interface IEstablishmentFacetValue : IFacetValue
{
    public int Id { get; }
    public FacetType Type { get; }
    public int EstablishmentId { get; }
}

public class EstablishmentFacetValue<T> : IEstablishmentFacetValue
{
    public int Id { get; set; }
    public T Value { get; set; }
    public int FacetId { get; set; }
    public int EnvironmentId { get; set; }
    public int EstablishmentId { get; set; }
    public FacetIdentifier Facet { get; set; }
    public FacetType Type { get; set; }
}

public class EstablishmentFacetAdvancedCriterion : AdvancedCriterion<IEstablishmentFacetValue>
{
    public FacetIdentifier Identifier { get; set; }
    public IEnvironmentFacetCriterion Value { get; set; }
    public override IQueryableExpressionBuilder<IEstablishmentFacetValue> GetExpressionBuilder(IQueryableExpressionBuilderFactory factory)
        => factory.Create(this);

}

public class EstablishmentFacetsAdvancedCriterion : EstablishmentFacetAdvancedCriterion, IListCriterion
{
    public ItemsMatching ItemsMatched { get; set; }
}
