using AdvancedFilters.Domain.Facets;
using AdvancedFilters.Infra.Storage.DAO;

namespace AdvancedFilters.Infra.Services.Sync.Dtos.Facets
{
    internal interface IEnvDaoCreator
    {
        EnvironmentFacetValueDao Create(int envId, Facet facet);
    }

    internal class EnvDaoCreator<T> : IEnvDaoCreator
    {
        T Value { get; }

        public EnvDaoCreator(T value)
        {
            Value = value;
        }

        public EnvironmentFacetValueDao Create(int environmentId, Facet facet)
        {
            var dao = new EnvironmentFacetValueDao
            {
                EnvironmentId = environmentId,
                FacetId = facet.Id,
                Facet = facet,
            };
            return dao.Fill(Value);
        }
    }
}
