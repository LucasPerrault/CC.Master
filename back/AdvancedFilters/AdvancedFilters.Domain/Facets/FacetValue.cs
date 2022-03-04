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
    [JsonConverter(typeof(EnvironmentFacetsCriterionConverter))]
    public IEnvironmentFacetCriterion Value { get; set; }
    public override IQueryableExpressionBuilder<IEnvironmentFacetValue> GetExpressionBuilder(IQueryableExpressionBuilderFactory factory)
        => factory.Create(this);

}
public class EnvironmentFacetsCriterionConverter : JsonConverter<IEnvironmentFacetCriterion>
{
    public override IEnvironmentFacetCriterion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        SerializerHelper.TryFind(jsonDocument, nameof(IEnvironmentFacetCriterion.Type), out var typeProperty);

        if (!Enum.TryParse<FacetType>(typeProperty.GetString(), true, out var facetType))
            throw new JsonException();

        var environmentFacetType = GetEnvironmentFacetType(facetType);

        var jsonObject = jsonDocument.RootElement.GetRawText();
        var result = (IEnvironmentFacetCriterion)JsonSerializer.Deserialize(jsonObject, environmentFacetType, options);

        return result;
    }

    public override void Write(Utf8JsonWriter writer, IEnvironmentFacetCriterion value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }

    private Type GetEnvironmentFacetType(FacetType type) => type switch
    {
        FacetType.Integer => typeof(SingleFacetValueComparisonCriterion<int>),
        FacetType.DateTime => typeof(SingleFacetValueComparisonCriterion<DateTime>),
        FacetType.Decimal => typeof(SingleFacetValueComparisonCriterion<decimal>),
        FacetType.Percentage => typeof(SingleFacetValueComparisonCriterion<decimal>),
        FacetType.String => typeof(SingleFacetValueComparisonCriterion<string>),
        _ => throw new JsonException()
    };
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
