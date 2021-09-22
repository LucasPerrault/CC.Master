import { Injectable } from '@angular/core';

import { IComparisonFilterCriterionForm } from '../components/comparison-filter-criterion';
import { LogicalOperator } from '../enums/logical-operator.enum';
import {
  AdvancedFilter,
  AdvancedFilterCriterion,
  AdvancedFilterTypeMapping,
} from '../models/advanced-filter.interface';
import { IAdvancedFilterConfiguration } from '../models/advanced-filter-configuration.interface';

@Injectable()
export class AdvancedFilterApiMappingService {
  public toAdvancedFilter(logicalOperator: LogicalOperator, criterionForms: IComparisonFilterCriterionForm[], configuration: IAdvancedFilterConfiguration): AdvancedFilter {
    const filtersCriterion: AdvancedFilter[] = criterionForms
      .filter((form: IComparisonFilterCriterionForm) => !!form.criterion && !!form.operator && !!form.values)
      .map(form => this.toAdvancedCriterionFilter(form, configuration));

    return !!logicalOperator
      ? AdvancedFilterTypeMapping.toFilterCombination(logicalOperator, filtersCriterion)
      : filtersCriterion[0];
  }

  public toAdvancedCriterionFilter(form: IComparisonFilterCriterionForm, configuration: IAdvancedFilterConfiguration): AdvancedFilter {
    const criterions = this.getFilterCriterions(form, configuration);

    return criterions.length > 1
      ? AdvancedFilterTypeMapping.toFilterCombination(LogicalOperator.And, criterions)
      : criterions[0];
  }

  private getFilterCriterions(
    form: IComparisonFilterCriterionForm,
    configuration: IAdvancedFilterConfiguration,
  ): AdvancedFilterCriterion[] {
    const filterKey = form.criterion.key;
    const operator = form.operator.id;
    const values = form.values[filterKey];

    const mapping = configuration.criterions.find(c => c.key === filterKey)?.mapping;
    const queries = mapping?.toApiQueries(values) || [];

    const comparisons = queries.map(q => AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, q));
    return comparisons.map(c => AdvancedFilterTypeMapping.toFilterCriterion(mapping.toFilterCriterion(c)));
  }
}
