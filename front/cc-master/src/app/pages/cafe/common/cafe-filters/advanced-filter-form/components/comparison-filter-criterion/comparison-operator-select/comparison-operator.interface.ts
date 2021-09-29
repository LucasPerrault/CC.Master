import { ComparisonOperator } from '../../../enums/comparison-operator.enum';

export interface IComparisonOperator {
  id: ComparisonOperator;
  name: string;
}

export const criterionOperators: IComparisonOperator[] = [
  {
    id: ComparisonOperator.Equals,
    name: 'cafe_criterionOperator_equal',
  },
  {
    id: ComparisonOperator.DoesNotEqual,
    name: 'cafe_criterionOperator_notEqual',
  },
  {
    id: ComparisonOperator.Contains,
    name: 'cafe_criterionOperator_contains',
  },
  {
    id: ComparisonOperator.DoesNotContain,
    name: 'cafe_criterionOperator_doesNotContain',
  },
  {
    id: ComparisonOperator.Between,
    name: 'cafe_criterionOperator_between',
  },
  {
    id: ComparisonOperator.StartsWith,
    name: 'cafe_criterionOperator_startsWith',
  },
  {
    id: ComparisonOperator.TrueOnly,
    name: 'cafe_criterionOperator_trueOnly',
  },
  {
    id: ComparisonOperator.FalseOnly,
    name: 'cafe_criterionOperator_falseOnly',
  },
  {
    id: ComparisonOperator.Until,
    name: 'cafe_criterionOperator_until',
  },
  {
    id: ComparisonOperator.Since,
    name: 'cafe_criterionOperator_since',
  },
  {
    id: ComparisonOperator.GreaterThan,
    name: 'cafe_criterionOperator_greaterThan',
  },
  {
    id: ComparisonOperator.SmallerThan,
    name: 'cafe_criterionOperator_smallerThan',
  },
];

export const getCriterionOperator = (id: ComparisonOperator): IComparisonOperator =>
  criterionOperators.find(c => c.id === id);
