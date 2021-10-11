import { IAdvancedFilterForm } from '../advanced-filter-form.interface';
import { IComparisonFilterCriterionForm } from '../components/comparison-filter-criterion';
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

  public static toComparisonFilterCriterion(operator: ComparisonOperator, value: string | number): IComparisonFilterCriterion {
    return { operator, value };
  }

  public static combine(criterions: AdvancedFilterCriterion[], operator: LogicalOperator) {
    return !!criterions.length && criterions.length > 1
        ? AdvancedFilterTypeMapping.toFilterCombination(operator, criterions)
        : criterions[0];
  }

  public static toCriterions(
      operator: ComparisonOperator,
      values: string[] | number[],
      toIFilterCriterion: (c: IComparisonFilterCriterion) => IFilterCriterion,
  ): AdvancedFilterCriterion[] {
    return values.map(v => this.toCriterion(operator, v, toIFilterCriterion));
  }

  public static toCriterion(
      operator: ComparisonOperator,
      value: string | number,
      toIFilterCriterion: (c: IComparisonFilterCriterion) => IFilterCriterion,
  ): AdvancedFilterCriterion {
    const comparisonFilterCriterion = AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, value);
    const filterCriterion = toIFilterCriterion(comparisonFilterCriterion);
    return AdvancedFilterTypeMapping.toFilterCriterion(filterCriterion);
  }

}
