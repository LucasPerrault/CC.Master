import { Injectable } from '@angular/core';

import {
  AdvancedFilter,
  AdvancedFilterFormMapping,
  AdvancedFilterTypeMapping, cast, IAdvancedCriterionAttributes,
  IAdvancedFilterForm, IComparisonCriterionAttributes,
} from '../../../common/components/advanced-filter-form';
import { AdvancedFilterOperatorMapping } from '../../../common/components/advanced-filter-form';
import { EnvironmentAdvancedFilterApiMappingService } from '../../../environments/advanced-filter';
import { SpeContactCriterionKey } from './specialized-contact-criterion-key.enum';

@Injectable()
export class SpecializedContactAdvancedFilterApiMappingService {
  constructor(private environmentApiMapping: EnvironmentAdvancedFilterApiMappingService) {}

  public toAdvancedFilter(advancedFilterForm: IAdvancedFilterForm): AdvancedFilter {
    return AdvancedFilterFormMapping.toAdvancedFilter(advancedFilterForm, a => this.getAdvancedFilter(a));
  }

  private getAdvancedFilter(attributes: IAdvancedCriterionAttributes): AdvancedFilter {
    switch (attributes.criterionKey) {
      case SpeContactCriterionKey.Role:
        return this.getRoleAdvancedFilter(cast<IComparisonCriterionAttributes>(attributes.content));
      case SpeContactCriterionKey.Environment:
        return this.getEnvironmentAdvancedFilter(cast<IAdvancedCriterionAttributes>(attributes.content));
    }
  }

  private getRoleAdvancedFilter(attributes: IComparisonCriterionAttributes): AdvancedFilter {
    const roleCodes = attributes.value[attributes.filterKey];
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => ({ roleCode: c });

    return AdvancedFilterTypeMapping.toAdvancedFilter(roleCodes, operator, toFilterCriterion, logicalOperator );
  }

  private getEnvironmentAdvancedFilter(attributes: IAdvancedCriterionAttributes): AdvancedFilter {
    const toFilterCriterion = criterion => ({ environment: criterion });
    return this.environmentApiMapping.getAdvancedFilter(attributes, toFilterCriterion);
  }
}
