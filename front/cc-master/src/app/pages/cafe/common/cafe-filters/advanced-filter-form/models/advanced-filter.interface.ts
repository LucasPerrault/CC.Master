import { LogicalOperator } from '../enums/logical-operator.enum';
import { IComparisonFilterCriterionEncapsulation } from './advanced-filter-mapping.interface';

export enum AdvancedFilterType {
  Logical = 'LogicalOperator',
  Criterion = 'Criterion',
}

export enum ComparisonOperatorDto {
  Equals = 'Equals',
  NotEquals = 'NotEquals',
  StrictlyGreaterThan = 'StrictlyGreaterThan',
  StrictlyLessThan = 'StrictlyLessThan',
}

export enum ItemsMatchedDto {
  All = 'All',
  Any = 'Any',
}

interface IFilter {
  filterElementType: AdvancedFilterType;
}

interface IFilterCombination {
  operator: LogicalOperator;
  values: IFilter[];
}

export interface IComparisonFilterCriterion {
  operator: ComparisonOperatorDto;
  value?: string | number | boolean;
}

export interface IFilterCriterion {
  [key: string]: IComparisonFilterCriterion | IFilterCriterion | IListFilterCriterion;
}

export interface IListFilterCriterion {
  [key: string]: IComparisonFilterCriterion | IFilterCriterion | IListFilterCriterion | ItemsMatchedDto;
  itemsMatched: ItemsMatchedDto;
}

export type AdvancedFilterCriterion = IFilter & IFilterCriterion;
export type AdvancedFilterCombination = IFilter & IFilterCombination;
export type AdvancedFilter = AdvancedFilterCriterion | AdvancedFilterCombination;

export class AdvancedFilterTypeMapping {
  public static toAdvancedFilter(
    values: string[] | number[] | boolean[],
    operator: ComparisonOperatorDto,
    toIFilterCriterion: IComparisonFilterCriterionEncapsulation,
    logicalOperator: LogicalOperator,
  ): AdvancedFilter {
    const criterions = values.map(v => AdvancedFilterTypeMapping.toCriterion(operator, v, toIFilterCriterion));
    return AdvancedFilterTypeMapping.combine(criterions, logicalOperator);
  }

  public static toFilterCombination(operator: LogicalOperator, values: AdvancedFilter[]): AdvancedFilterCombination {
    return { filterElementType: AdvancedFilterType.Logical, operator, values } as AdvancedFilterCombination;
  };

  public static toFilterCriterion(criterion: IFilterCriterion | IListFilterCriterion): AdvancedFilterCriterion {
    return { filterElementType: AdvancedFilterType.Criterion, ...criterion } as AdvancedFilterCriterion;
  };

  public static toComparisonFilterCriterion(operator: ComparisonOperatorDto, value: string | number | boolean): IComparisonFilterCriterion {
    return { operator, value };
  }

  public static combine(criterions: AdvancedFilterCriterion[], operator: LogicalOperator) {
    return !!criterions.length && criterions.length > 1
        ? AdvancedFilterTypeMapping.toFilterCombination(operator, criterions)
        : criterions[0];
  }

  private static toCriterion(
    operator: ComparisonOperatorDto,
    value: string | number | boolean,
    toIFilterCriterion: IComparisonFilterCriterionEncapsulation,
  ): AdvancedFilterCriterion {
    const comparisonFilterCriterion = AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, value);
    const filterCriterion = toIFilterCriterion(comparisonFilterCriterion);
    return AdvancedFilterTypeMapping.toFilterCriterion(filterCriterion);
  }
}
