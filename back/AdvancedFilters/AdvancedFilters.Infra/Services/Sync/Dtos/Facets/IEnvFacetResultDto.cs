using AdvancedFilters.Domain.Facets;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedFilters.Infra.Services.Sync.Dtos.Facets
{
    internal interface IEnvFacetResultDto : IFacetResultDto
    {
        IReadOnlyCollection<IEnvDaoCreator> DaoCreators { get; }
    }

    internal class EnvironmentSingleResultDto<T> : IEnvFacetResultDto, IFacetSingleValue<T>
    {
        public FacetType Type { get; internal set; } // TODO enlever internal set; ?
        public T Value { get; internal set; }

        public IReadOnlyCollection<IEnvDaoCreator> DaoCreators
            => new List<IEnvDaoCreator> { new EnvDaoCreator<T>(Value) };
    }

    internal class EnvironmentMultipleResultDto<T> : IEnvFacetResultDto, IFacetMultipleValues<T>
    {
        public FacetType Type { get; }
        public List<T> Values { get; }

        public IReadOnlyCollection<IEnvDaoCreator> DaoCreators
            => Values.Select(v => new EnvDaoCreator<T>(v)).ToList();
    }
}
