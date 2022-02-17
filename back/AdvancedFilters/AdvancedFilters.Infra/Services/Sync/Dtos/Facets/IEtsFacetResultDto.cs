using AdvancedFilters.Domain.Facets;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedFilters.Infra.Services.Sync.Dtos.Facets
{
    internal interface IEtsFacetResultDto : IFacetResultDto
    {
        IReadOnlyCollection<IEtsDaoCreator> DaoCreators { get; }
    }

    internal abstract class EstablishmentFacetResultDto<TValue> : IEtsFacetResultDto where TValue : IEstablishmentValue
    {
        public FacetType Type { get; }
        public IReadOnlyCollection<TValue> Establishments { get; }

        public IReadOnlyCollection<IEtsDaoCreator> DaoCreators
            => Establishments.SelectMany(e => e.EstablishmentDaoCreators).ToList();
    }

    internal interface IEstablishmentValue
    {
        public int EstablishmentId { get; set; }

        public IReadOnlyCollection<IEtsDaoCreator> EstablishmentDaoCreators { get; }
    }

    internal class EstablishmentSingleValue<T> : IEstablishmentValue, IFacetSingleValue<T>
    {
        public int EstablishmentId { get; set; }
        public T Value { get; set; }

        public IReadOnlyCollection<IEtsDaoCreator> EstablishmentDaoCreators
            => new List<IEtsDaoCreator> { new EtsDaoCreator<T>(Value, EstablishmentId) };
    }

    internal class EstablishmentMultipleValue<T> : IEstablishmentValue, IFacetMultipleValues<T>
    {
        public int EstablishmentId { get; set; }
        public List<T> Values { get; set; }

        public IReadOnlyCollection<IEtsDaoCreator> EstablishmentDaoCreators
            => Values.Select(v => new EtsDaoCreator<T>(v, EstablishmentId)).ToList();
    }
}
