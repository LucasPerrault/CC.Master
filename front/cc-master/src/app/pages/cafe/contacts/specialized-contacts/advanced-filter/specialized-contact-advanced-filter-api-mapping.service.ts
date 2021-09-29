import { Injectable } from '@angular/core';

import {
  AdvancedFilter,
  AdvancedFilterTypeMapping,
  ComparisonOperator,
  getComparisonBooleanValue,
  IAdvancedFilterForm,
  IComparisonFilterCriterionForm,
  IComparisonValue,
} from '../../../common/cafe-filters/advanced-filter-form';
import { SpeContactAdvancedFilterKey } from './specialized-contact-advanced-filter-key.enum';

interface IAdvancedFilterAttributes {
  filterKey: string;
  operator: ComparisonOperator;
  value?: IComparisonValue;
}

const toAdvancedFilterAttributes = (form: IComparisonFilterCriterionForm): IAdvancedFilterAttributes =>
  ({ filterKey: form?.criterion?.key, operator: form?.operator?.id, value: form?.values });

@Injectable()
export class SpecializedContactAdvancedFilterApiMappingService {
  public toAdvancedFilter(advancedFilterForm: IAdvancedFilterForm): AdvancedFilter {
    if (!advancedFilterForm?.logicalOperator && !advancedFilterForm?.criterionForms?.length) {
      return;
    }

    const filtersCriterion: AdvancedFilter[] = advancedFilterForm.criterionForms
      .map(form => toAdvancedFilterAttributes(form))
      .filter(attributes => !!attributes?.filterKey && !!attributes?.operator)
      .map(attributes => this.getAdvancedFilter(attributes));

    if (!advancedFilterForm.logicalOperator || filtersCriterion.length === 1) {
      return filtersCriterion[0];
    }

    return AdvancedFilterTypeMapping.toFilterCombination(advancedFilterForm.logicalOperator.id, filtersCriterion);
  }

  private getAdvancedFilter(attributes: IAdvancedFilterAttributes): AdvancedFilter {
    switch (attributes.filterKey) {
      case SpeContactAdvancedFilterKey.IsConfirmed:
        return this.getIsConfirmedAdvancedFilter(attributes.operator);
      case SpeContactAdvancedFilterKey.Subdomain:
        const subdomain = attributes.value[attributes.filterKey];
        return this.getSubdomainAdvancedFilter(attributes.operator, subdomain);
    }
  }

  private getIsConfirmedAdvancedFilter(operator: ComparisonOperator): AdvancedFilter {
    const query = getComparisonBooleanValue(operator);
    const comparison = AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, query);

    return AdvancedFilterTypeMapping.toFilterCriterion({
      isConfirmed: comparison,
    });
  }

  private getSubdomainAdvancedFilter(operator: ComparisonOperator, subdomain?: string): AdvancedFilter {
    const comparison = AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, subdomain);

    return AdvancedFilterTypeMapping.toFilterCriterion({
      subdomain: comparison,
    });
  }
}
