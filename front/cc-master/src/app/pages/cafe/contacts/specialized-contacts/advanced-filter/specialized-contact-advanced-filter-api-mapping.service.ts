import { Injectable } from '@angular/core';
import { IEnvironment } from '@cc/domain/environments';

import {
  AdvancedFilter,
  AdvancedFilterTypeMapping,
  ComparisonOperator,
  getComparisonBooleanValue,
  IAdvancedFilterForm,
  IComparisonFilterCriterionForm,
  IComparisonValue, LogicalOperator,
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
        const subdomains = attributes.value[attributes.filterKey];
        return this.getSubdomainAdvancedFilter(attributes.operator, subdomains);
    }
  }

  private getIsConfirmedAdvancedFilter(operator: ComparisonOperator): AdvancedFilter {
    const query = getComparisonBooleanValue(operator);
    const comparison = AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, query);

    return AdvancedFilterTypeMapping.toFilterCriterion({
      isConfirmed: comparison,
    });
  }

  private getSubdomainAdvancedFilter(operator: ComparisonOperator, subdomains: IEnvironment[]): AdvancedFilter {
    const queries = subdomains.map(a => `${ a.subDomain }`);
    const comparisons = queries.map(q => AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, q));

    const criterions = comparisons.map(c => AdvancedFilterTypeMapping.toFilterCriterion({
      subdomain: c,
    }));

    return !!criterions.length && criterions.length > 1
      ? AdvancedFilterTypeMapping.toFilterCombination(LogicalOperator.And, criterions)
      : criterions[0];
  }
}
