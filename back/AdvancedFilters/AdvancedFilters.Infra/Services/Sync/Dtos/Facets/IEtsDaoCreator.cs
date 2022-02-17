using AdvancedFilters.Domain.Facets;
using AdvancedFilters.Infra.Storage.DAO;

namespace AdvancedFilters.Infra.Services.Sync.Dtos.Facets
{
    internal interface IEtsDaoCreator
    {
        EstablishmentFacetValueDao Create(int envId, Facet facet);
    }

    internal class EtsDaoCreator<T> : IEtsDaoCreator
    {
        T Value { get; }
        public int EstablishmentId { get; set; }

        public EtsDaoCreator(T value, int etsId)
        {
            Value = value;
            EstablishmentId = etsId;
        }

        public EstablishmentFacetValueDao Create(int envId, Facet facet)
        {
            var dao = new EstablishmentFacetValueDao
            {
                EnvironmentId = envId,
                EstablishmentId = EstablishmentId,
                Facet = facet,
            };
            return dao.Fill(Value);
        }
    }
}
