using AdvancedFilters.Domain.Filters.Builders;
using AdvancedFilters.Domain.Filters.Models;
using System;
using System.Diagnostics;
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
    //[JsonConverter(typeof(EnvironmentFacetsCriterionConverter))]
    public IEnvironmentFacetCriterion Value { get; set; }
    public override IQueryableExpressionBuilder<IEnvironmentFacetValue> GetExpressionBuilder(IQueryableExpressionBuilderFactory factory)
        => factory.Create(this);

}
//public class EnvironmentFacetsCriterionConverter : JsonConverter<IEnvironmentFacetCriterion>
//{

//    public override IEnvironmentFacetCriterion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//    {
//        Console.WriteLine($"TokenType={reader.TokenType}");
//        IEnvironmentFacetCriterion result = null;
//        ComparisonOperators comparison = ComparisonOperators.Equals;
//        FacetType facetType = FacetType.Unknown;
//        int facetValue = 0;

//        while (reader.Read())
//        {
//            if (reader.TokenType != JsonTokenType.PropertyName)
//                break;

//            var propName = reader.GetString();
//            reader.Read();

//            switch (char.ToUpper(propName[0]) + propName.Substring(1))
//            {
//                case nameof(IEnvironmentFacetCriterion.Operator):
//                    {
//                        var value = reader.GetString();
//                        comparison = (ComparisonOperators)Enum.Parse(typeof(ComparisonOperators), value);
//                        break;
//                    }
//                case nameof(IEnvironmentFacetCriterion.Type):
//                    {
//                        var value = reader.GetString();
//                        facetType = (FacetType)Enum.Parse(typeof(FacetType), value); // TODO
//                    }

//                    break;
//                case "Value":
//                    {
//                        facetValue = reader.GetInt32();
//                    }
//                    break;
//            }
//        }

//        if (facetType == FacetType.Integer)
//            return new SingleFacetValueComparisonCriterion<int> { Value = facetValue, Operator = comparison, Type = facetType };

//        return result;
//    }

//    public override void Write(Utf8JsonWriter writer, IEnvironmentFacetCriterion value, JsonSerializerOptions options)
//    {
//        throw new NotImplementedException();
//    }
//}
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
