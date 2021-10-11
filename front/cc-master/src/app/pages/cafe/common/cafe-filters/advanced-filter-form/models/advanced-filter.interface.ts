import { ComparisonOperator } from '../enums/comparison-operator.enum';
import { LogicalOperator } from '../enums/logical-operator.enum';

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
  value?: string | number;
}

export interface IFilterCriterion {
  [key: string]: IComparisonFilterCriterion | IFilterCriterion;
}

export type AdvancedFilterCriterion = IFilter & IFilterCriterion;
export type AdvancedFilterCombination = IFilter & IFilterCombination;
export type AdvancedFilter = AdvancedFilterCriterion | AdvancedFilterCombination;

export class AdvancedFilterTypeMapping {
  public static toFilterCombination(operator: LogicalOperator, values: AdvancedFilter[]): AdvancedFilterCombination {
    return { filterElementType: AdvancedFilterType.Logical, operator, values } as AdvancedFilterCombination;
  };

  public static toFilterCriterion(criterion: IFilterCriterion): AdvancedFilterCriterion {
    return { filterElementType: AdvancedFilterType.Criterion, ...criterion } as AdvancedFilterCriterion;
  };

  public static toComparisonFilterCriterion(operator: ComparisonOperator, value: string): IComparisonFilterCriterion {
    return { operator, value };
  }

  public static toComparisonFilterNumberCriterion(operator: ComparisonOperator, value: number): IComparisonFilterCriterion {
    return { operator, value };
  }
}
