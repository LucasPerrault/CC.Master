using AdvancedFilters.Domain.Billing.Models;
using AdvancedFilters.Domain.Contacts.Models;
using AdvancedFilters.Domain.Facets;
using AdvancedFilters.Domain.Filters.Builders;
using AdvancedFilters.Domain.Filters.Models;
using AdvancedFilters.Domain.Instance.Models;
using AdvancedFilters.Infra.Filters.Builders.Chaining;
using AdvancedFilters.Infra.Filters.Builders.Implementations;
using Storage.Infra.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Environment = AdvancedFilters.Domain.Instance.Models.Environment;

namespace AdvancedFilters.Infra.Filters.Builders
{
    internal static class QueryableExpressionBuilderExtensions
    {
        public static IQueryableExpressionBuilder<TValue> GetExpressionBuilderOrBypass<TValue>(this AdvancedCriterion<TValue> criterion, IQueryableExpressionBuilderFactory factory)
        {
            return criterion?.GetExpressionBuilder(factory)
                ?? new BypassExpressionBuilder<TValue>();
        }
    }

    public interface IAdvancedExpressionChainer
    {
        Expression<Func<TItem, bool>> ChainToPropertyList<TItem, TProperty>
        (
            AdvancedCriterion<TProperty> criterion,
            Expression<Func<TItem, IEnumerable<TProperty>>> expression
        );

        Expression<Func<TItem, bool>> ChainToPropertyItem<TItem, TProperty>
        (
            AdvancedCriterion<TProperty> criterion,
            Expression<Func<TItem, TProperty>> expression
        );
    }

    public class AdvancedCriterionExpressionBuilderFactory : IQueryableExpressionBuilderFactory, IAdvancedExpressionChainer
    {
        private readonly IFacetsStore _facetsStore;

        public AdvancedCriterionExpressionBuilderFactory(IFacetsStore facetsStore)
        {
            _facetsStore = facetsStore;
        }

        public IQueryableExpressionBuilder<Environment> Create(EnvironmentAdvancedCriterion criterion)
        {
            return new EnvironmentCriterionExpressionBuilder(criterion, this, _facetsStore);
        }

        public IQueryableExpressionBuilder<Establishment> Create(EstablishmentAdvancedCriterion criterion)
        {
            return new EstablishmentCriterionExpressionBuilder(criterion, this);
        }

        public IQueryableExpressionBuilder<AppInstance> Create(AppInstanceAdvancedCriterion criterion)
        {
            return new AppInstanceCriterionExpressionBuilder(criterion, this);
        }

        public IQueryableExpressionBuilder<LegalUnit> Create(LegalUnitAdvancedCriterion criterion)
        {
            return new LegalUnitCriterionExpressionBuilder(criterion, this);
        }

        public IQueryableExpressionBuilder<Distributor> Create(DistributorAdvancedCriterion criterion)
        {
            return new DistributorCriterionExpressionBuilder(criterion, this);
        }

        public IQueryableExpressionBuilder<AppContact> Create(AppContactAdvancedCriterion criterion)
        {
            return new AppContactCriterionExpressionBuilder(criterion, this);
        }

        public IQueryableExpressionBuilder<ClientContact> Create(ClientContactAdvancedCriterion criterion)
        {
            return new ClientContactCriterionExpressionBuilder(criterion, this);
        }

        public IQueryableExpressionBuilder<SpecializedContact> Create(SpecializedContactAdvancedCriterion criterion)
        {
            return new SpecializedContactCriterionExpressionBuilder(criterion, this);
        }

        public IQueryableExpressionBuilder<Contract> Create(ContractAdvancedCriterion criterion)
        {
            return new ContractCriterionExpressionBuilder(criterion, this);
        }

        public IQueryableExpressionBuilder<Client> Create(ClientAdvancedCriterion criterion)
        {
            return new ClientCriterionExpressionBuilder(criterion, this);
        }

        public IQueryableExpressionBuilder<IEnvironmentFacetValue> Create(EnvironmentFacetAdvancedCriterion criterion)
        {
            return new FacetCriterionExpressionBuilder(criterion, this, _facetsStore);
        }

        public Expression<Func<TItem, bool>> ChainToPropertyList<TItem, TProperty>
        (
            AdvancedCriterion<TProperty> criterion,
            Expression<Func<TItem, IEnumerable<TProperty>>> expression
        )
        {
            var builder = criterion.GetExpressionBuilderOrBypass(this);
            return expression.Chain(ForEachApplyToItem(builder));
        }

        public Expression<Func<TItem, bool>> ChainToPropertyItem<TItem, TProperty>
        (
            AdvancedCriterion<TProperty> criterion,
            Expression<Func<TItem, TProperty>> expression
        )
        {
            var builder = criterion.GetExpressionBuilderOrBypass(this);
            return expression.Chain(ApplyToItem(builder));
        }

        private Expression<Func<IEnumerable<TProperty>, bool>> ForEachApplyToItem<TProperty>(IQueryableExpressionBuilder<TProperty> expressionBuilder)
        {
            if (!expressionBuilder.CanBuild())
            {
                return _ => true;
            }

            return expressionBuilder.IntersectionOrBypass;
        }

        private Expression<Func<TProperty, bool>> ApplyToItem<TProperty>(IQueryableExpressionBuilder<TProperty> expressionBuilder)
        {
            if (!expressionBuilder.CanBuild())
            {
                return _ => true;
            }

            return expressionBuilder.MatchOrBypass;
        }
    }
}
