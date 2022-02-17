using AdvancedFilters.Domain.Facets;
using System.Collections.Generic;

namespace AdvancedFilters.Infra.Services.Sync.Dtos.Facets
{
    internal interface IFacetDto
    {
        FacetScope Scope { get; }
        string Code { get; }
        string CultureKey { get; }

        FacetType Type { get; }
    }

    internal class EnvFacetDto : FacetDto<IEnvFacetResultDto> { }
    internal class EtsFacetDto : FacetDto<IEtsFacetResultDto> { }

    internal abstract class FacetDto<TResult> : IFacetDto
        where TResult : IFacetResultDto
    {
        public FacetScope Scope { get; internal set; } // TODO enlever le internal set;
        public string Code { get; internal set; }
        public string CultureKey { get; internal set; }
        public TResult Result { get; internal set; }

        public FacetType Type => Result.Type;
    }

    internal interface IFacetResultDto
    {
        FacetType Type { get; }
    }

    internal interface IFacetSingleValue<T>
    {
        public T Value { get; }
    }

    internal interface IFacetMultipleValues<T>
    {
        public List<T> Values { get; }
    }
}
