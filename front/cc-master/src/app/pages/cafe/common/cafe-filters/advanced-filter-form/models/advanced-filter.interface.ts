import { ComparisonOperator } from '../enums/comparison-operator.enum';
import { LogicalOperator } from '../enums/logical-operator.enum';
import { IComparisonFilterCriterionEncapsulation } from './advanced-filter-mapping.interface';

export enum AdvancedFilterType {
  Logical = 'LogicalOperator',
  Criterion = 'Criterion',
}

interface IFilter {
  filterElementType: AdvancedFilterType;
}

interface IFilterCombination {
  operator: LogicalOperator;
  values: IFilter[];
}

export interface IComparisonFilterCriterion {
  operator: ComparisonOperator;
  value?: string | number | boolean;
}

export interface IFilterCriterion {
  [key: string]: IComparisonFilterCriterion | IFilterCriterion;
}

export type AdvancedFilterCriterion = IFilter & IFilterCriterion;
export type AdvancedFilterCombination = IFilter & IFilterCombination;
export type AdvancedFilter = AdvancedFilterCriterion | AdvancedFilterCombination;

export class AdvancedFilterTypeMapping {
  public static toAdvancedFilter(
    values: string[] | number[] | boolean[],
    operator: ComparisonOperator,
    toIFilterCriterion: IComparisonFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const criterions = values.map(v => this.toCriterion(operator, v, toIFilterCriterion));
    return AdvancedFilterTypeMapping.combine(criterions, LogicalOperator.Or);
  }

  public static toFilterCombination(operator: LogicalOperator, values: AdvancedFilter[]): AdvancedFilterCombination {
    return { filterElementType: AdvancedFilterType.Logical, operator, values } as AdvancedFilterCombination;
  };

  public static toFilterCriterion(criterion: IFilterCriterion): AdvancedFilterCriterion {
    return { filterElementType: AdvancedFilterType.Criterion, ...criterion } as AdvancedFilterCriterion;
  };

  public static toComparisonFilterCriterion(operator: ComparisonOperator, value: string | number | boolean): IComparisonFilterCriterion {
    return { operator, value };
  }

  public static combine(criterions: AdvancedFilterCriterion[], operator: LogicalOperator) {
    return !!criterions.length && criterions.length > 1
        ? AdvancedFilterTypeMapping.toFilterCombination(operator, criterions)
        : criterions[0];
  }

  public static toCriterion(
    operator: ComparisonOperator,
    value: string | number | boolean,
    toIFilterCriterion: IComparisonFilterCriterionEncapsulation,
  ): AdvancedFilterCriterion {
    const comparisonFilterCriterion = AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, value);
    const filterCriterion = toIFilterCriterion(comparisonFilterCriterion);
    return AdvancedFilterTypeMapping.toFilterCriterion(filterCriterion);
  }

}
