using AdvancedFilters.Domain.Facets;
using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Filters.Builders.Chaining;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AdvancedFilters.Infra.Filters.Builders.Implementations
{
    internal class EstablishmentCriterionExpressionBuilder : AdvancedCriterionExpressionBuilder<Establishment, EstablishmentAdvancedCriterion>
    {
        private readonly IFacetsStore _facetsStore;

        public EstablishmentCriterionExpressionBuilder(EstablishmentAdvancedCriterion criterion, IAdvancedExpressionChainer chainer, IFacetsStore facetsStore)
            : base(criterion, chainer)
        {
            _facetsStore = facetsStore;
        }

        protected override IEnumerable<IPropertyExpressionBuilder<Establishment>> GetCriteria()
        {
            yield return Apply(Criterion.Environment).To(e => e.Environment);
            yield return Apply(Criterion.LegalUnit).To(e => e.LegalUnit);
            yield return new EstablishmentFacetsPropertyExpressionBuilder(Criterion.Facets, _facetsStore);
        }
    }


    internal class EstablishmentFacetCriterionExpressionBuilder : AdvancedCriterionExpressionBuilder<IEstablishmentFacetValue, EstablishmentFacetAdvancedCriterion>
    {
        private readonly IFacetsStore _store;

        public EstablishmentFacetCriterionExpressionBuilder(EstablishmentFacetAdvancedCriterion criterion, IAdvancedExpressionChainer chainer, IFacetsStore store)
            : base(criterion, chainer)
        {
            _store = store;
        }

        protected override IEnumerable<IPropertyExpressionBuilder<IEstablishmentFacetValue>> GetCriteria()
        {
            yield return new EstablishmentFacetValuePropertyExpressionBuilder(Criterion, _store);
        }
    }


    internal class EstablishmentFacetValuePropertyExpressionBuilder : IPropertyExpressionBuilder<IEstablishmentFacetValue>
    {
        private readonly EstablishmentFacetAdvancedCriterion _criterion;
        private readonly IFacetsStore _store;

        public EstablishmentFacetValuePropertyExpressionBuilder(EstablishmentFacetAdvancedCriterion criterion, IFacetsStore store)
        {
            _criterion = criterion;
            _store = store;
        }

        public Expression<System.Func<IEstablishmentFacetValue, bool>> ForItem()
        {
            return null;//_store.GetValuesQueryable(_criterion.);
        }

        public Expression<System.Func<IEnumerable<IEstablishmentFacetValue>, bool>> ForList(ItemsMatching matching)
        {
            return ForItem().ToExpressionForList(matching);
        }
    }

    internal class EstablishmentFacetsPropertyExpressionBuilder : IPropertyExpressionBuilder<Establishment>
    {
        private readonly EstablishmentFacetsAdvancedCriterion _criterion;
        private readonly IFacetsStore _store;

        public EstablishmentFacetsPropertyExpressionBuilder(EstablishmentFacetsAdvancedCriterion criterion, IFacetsStore store)
        {
            _criterion = criterion;
            _store = store;
        }

        public Expression<System.Func<Establishment, bool>> ForItem()
        {
            if (_criterion == null)
                return e => true;

            return _store.GetEstablishmentFacetFilter(_criterion);
        }

        public Expression<System.Func<IEnumerable<Establishment>, bool>> ForList(ItemsMatching matching)
        {
            return ForItem().ToExpressionForList(matching);
        }
    }
}
