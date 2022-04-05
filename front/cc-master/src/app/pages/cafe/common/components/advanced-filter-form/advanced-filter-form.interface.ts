import { IComparisonOperator, IComparisonValue, IFormlyFieldValue } from './components/advanced-criterion-form';
import { ILogicalOperator } from './components/logical-operator-select/logical-operator.interface';
import { ComparisonOperator } from './enums/comparison-operator.enum';
import { AdvancedFilter, AdvancedFilterTypeMapping } from './models/advanced-filter.interface';
import { ComparisonCriterion } from './models/comparison-criterion.interface';

export interface IAdvancedFilterForm {
  logicalOperator: ILogicalOperator;
  criterionForms: IAdvancedCriterionForm[];
}

type AdvancedCriterionFormContent = IAdvancedCriterionForm | IComparisonCriterionForm;
export interface IAdvancedCriterionForm {
  criterion: ComparisonCriterion;
  content: AdvancedCriterionFormContent;
}
const isCriterion = (content: AdvancedCriterionFormContent | AdvancedCriterionAttributesContent): boolean => 'criterion' in content;

export interface IComparisonCriterionForm {
  operator?: IComparisonOperator;
  values?: IComparisonValue;
}

type AdvancedCriterionAttributesContent = IAdvancedCriterionAttributes | IComparisonCriterionAttributes;
export const cast = <T extends AdvancedCriterionAttributesContent>(attributes: AdvancedCriterionAttributesContent): T =>
  attributes as T;

export interface IAdvancedCriterionAttributes {
  criterionKey: string;
  criterion: ComparisonCriterion;
  content: AdvancedCriterionAttributesContent;
}

export interface IComparisonCriterionAttributes {
  operator: ComparisonOperator;
  filterKey: string;
  value?: IFormlyFieldValue;
}

export class AdvancedFilterFormMapping {

  public static toAdvancedFilter(
      advancedFilterForm: IAdvancedFilterForm,
      getAdvancedFilter: (a: IAdvancedCriterionAttributes) => AdvancedFilter,
  ): AdvancedFilter {

    if (!advancedFilterForm?.logicalOperator && !advancedFilterForm?.criterionForms?.length) {
      return;
    }

    const attributes = advancedFilterForm.criterionForms.map(form => this.toAttributes(form));
    const validAttributes = attributes.filter(attribute => this.isValid(attribute));
    const filtersCriterion = validAttributes.map(attribute => getAdvancedFilter(attribute));

    if (!advancedFilterForm.logicalOperator || filtersCriterion.length === 1) {
      return filtersCriterion[0];
    }

    return AdvancedFilterTypeMapping.toFilterCombination(advancedFilterForm.logicalOperator.id, filtersCriterion);
  }

  private static toAttributes = (form: IAdvancedCriterionForm): IAdvancedCriterionAttributes => {
    if (!isCriterion(form.content)) {
      form.content = form.content as IComparisonCriterionForm;
      return {
        criterionKey: form.criterion?.key,
        criterion: form.criterion,
        content: {
          operator: form.content?.operator?.id,
          filterKey: form.content?.values?.key,
          value: form.content?.values?.fieldValues,
        },
      };
    }

    form.content = form.content as IAdvancedCriterionForm;
    return { criterionKey: form.criterion.key, criterion: form.criterion, content: AdvancedFilterFormMapping.toAttributes(form.content) };
  };

  private static isValid(attribute: IAdvancedCriterionAttributes): boolean {
    if (isCriterion(attribute.content)) {
      return !!(attribute.content as IAdvancedCriterionAttributes).criterion;
    }

    attribute.content = attribute.content as IComparisonCriterionAttributes;
    return !!attribute.content.filterKey && !!attribute.content.operator;
  }
}
