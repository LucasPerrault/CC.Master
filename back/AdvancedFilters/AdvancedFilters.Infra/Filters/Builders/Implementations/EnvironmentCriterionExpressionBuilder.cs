using AdvancedFilters.Domain.Facets;
using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Filters.Builders.Chaining;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Infra.Filters.Builders.Implementations
{
    internal class EnvironmentCriterionExpressionBuilder : AdvancedCriterionExpressionBuilder<Environment, EnvironmentAdvancedCriterion>
    {
        private readonly IFacetsStore _facetsStore;

        public EnvironmentCriterionExpressionBuilder(EnvironmentAdvancedCriterion criterion, IAdvancedExpressionChainer chainer, IFacetsStore facetsStore)
            : base(criterion, chainer)
        {
            _facetsStore = facetsStore;
        }

        protected override IEnumerable<IPropertyExpressionBuilder<Environment>> GetCriteria()
        {
            yield return Apply(Criterion.CreatedAt).To(e => e.CreatedAt);
            yield return Apply(Criterion.Subdomain).To(e => e.Subdomain);
            yield return Apply(Criterion.Cluster).To(e => e.Cluster);
            yield return ApplyMany(Criterion.LegalUnits).To(e => e.LegalUnits);
            yield return ApplyMany(Criterion.AppInstances).To(EnvironmentExpressions.AppInstancesAvailableForSelection);
            yield return ApplyMany(Criterion.Distributors).To(e => e.Accesses.Select(a => a.Distributor).Distinct());
            yield return ApplyMany(Criterion.Contracts).To(e => e.Contracts);
            yield return Apply(Criterion.DistributorType).To(EnvironmentExpressions.DistributorTypeFn);
            yield return new EnvFacetsPropertyExpressionBuilder(Criterion.Facets, _facetsStore);
        }
    }

    internal class FacetCriterionExpressionBuilder : AdvancedCriterionExpressionBuilder<IEnvironmentFacetValue, EnvironmentFacetAdvancedCriterion>
    {
        private readonly IFacetsStore _store;

        public FacetCriterionExpressionBuilder(EnvironmentFacetAdvancedCriterion criterion, IAdvancedExpressionChainer chainer, IFacetsStore store)
            : base(criterion, chainer)
        {
            _store = store;
        }

        protected override IEnumerable<IPropertyExpressionBuilder<IEnvironmentFacetValue>> GetCriteria()
        {
            yield return new EnvFacetValuePropertyExpressionBuilder(Criterion, _store);
        }
    }

    internal class EnvFacetValuePropertyExpressionBuilder : IPropertyExpressionBuilder<IEnvironmentFacetValue>
    {
        private readonly EnvironmentFacetAdvancedCriterion _criterion;
        private readonly IFacetsStore _store;

        public EnvFacetValuePropertyExpressionBuilder(EnvironmentFacetAdvancedCriterion criterion, IFacetsStore store)
        {
            _criterion = criterion;
            _store = store;
        }

        public Expression<System.Func<IEnvironmentFacetValue, bool>> ForItem()
        {
            return null;//_store.GetValuesQueryable(_criterion.);
        }

        public Expression<System.Func<IEnumerable<IEnvironmentFacetValue>, bool>> ForList(ItemsMatching matching)
        {
            return ForItem().ToExpressionForList(matching);
        }
    }

    internal class EnvFacetsPropertyExpressionBuilder : IPropertyExpressionBuilder<Environment>
    {
        private readonly EnvironmentFacetsAdvancedCriterion _criterion;
        private readonly IFacetsStore _store;

        public EnvFacetsPropertyExpressionBuilder(EnvironmentFacetsAdvancedCriterion criterion, IFacetsStore store)
        {
            _criterion = criterion;
            _store = store;
        }

        public Expression<System.Func<Environment, bool>> ForItem()
        {
            if (_criterion == null)
                return e => true;

            return _store.GetEnvFacetFilter(_criterion);
        }

        public Expression<System.Func<IEnumerable<Environment>, bool>> ForList(ItemsMatching matching)
        {
            return ForItem().ToExpressionForList(matching);
        }
    }
}
