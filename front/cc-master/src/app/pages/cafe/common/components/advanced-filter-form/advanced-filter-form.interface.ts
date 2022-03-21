import { IComparisonFilterCriterionForm, IComparisonValue } from './components/comparison-filter-criterion';
import { ILogicalOperator } from './components/logical-operator-select/logical-operator.interface';
import { ComparisonOperator } from './enums/comparison-operator.enum';
import { AdvancedFilter, AdvancedFilterTypeMapping } from './models/advanced-filter.interface';
import { ComparisonCriterion } from './models/comparison-criterion.interface';

export interface IAdvancedFilterForm {
  logicalOperator: ILogicalOperator;
  criterionForms: IComparisonFilterCriterionForm[];
}

export interface IAdvancedFilterAttributes {
  filterKey: string;
  criterion: ComparisonCriterion;
  operator: ComparisonOperator;
  value?: IComparisonValue;
}

export class AdvancedFilterFormMapping {

  public static toAdvancedFilter(
      advancedFilterForm: IAdvancedFilterForm,
      getAdvancedFilter: (a: IAdvancedFilterAttributes) => AdvancedFilter,
  ): AdvancedFilter {

    if (!advancedFilterForm?.logicalOperator && !advancedFilterForm?.criterionForms?.length) {
      return;
    }

    const filtersCriterion: AdvancedFilter[] = advancedFilterForm.criterionForms
        .map(form => this.toAdvancedFilterAttributes(form))
        .filter(attributes => !!attributes?.filterKey && !!attributes?.operator)
        .map(attributes => getAdvancedFilter(attributes));

    if (!advancedFilterForm.logicalOperator || filtersCriterion.length === 1) {
      return filtersCriterion[0];
    }

    return AdvancedFilterTypeMapping.toFilterCombination(advancedFilterForm.logicalOperator.id, filtersCriterion);
  }

  private static toAdvancedFilterAttributes = (form: IComparisonFilterCriterionForm): IAdvancedFilterAttributes => ({
    filterKey: form?.values?.key ?? form?.criterion?.key,
    criterion: form.criterion,
    operator: form?.operator?.id,
    value: form?.values,
  });
}
