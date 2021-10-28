import { Injectable } from '@angular/core';

import {
  AdvancedFilter,
  AdvancedFilterFormMapping,
  AdvancedFilterTypeMapping,
  IAdvancedFilterAttributes,
  IAdvancedFilterForm,
} from '../../../common/cafe-filters/advanced-filter-form';
import { AdvancedFilterOperatorMapping } from '../../../common/cafe-filters/advanced-filter-form';
import { EnvironmentAdvancedFilterApiMappingService } from '../../../environments/advanced-filter';
import { CommonApiMappingStrategies } from '../../common/advanced-filter/common-api-mapping-strategies';
import { SpeContactAdvancedFilterKey } from './specialized-contact-advanced-filter-key.enum';

@Injectable()
export class SpecializedContactAdvancedFilterApiMappingService {
  constructor(private environmentApiMapping: EnvironmentAdvancedFilterApiMappingService) {
  }

  public toAdvancedFilter(advancedFilterForm: IAdvancedFilterForm): AdvancedFilter {
    return AdvancedFilterFormMapping.toAdvancedFilter(advancedFilterForm, a => this.getAdvancedFilter(a));
  }

  private getAdvancedFilter(attributes: IAdvancedFilterAttributes): AdvancedFilter {
    switch (attributes.filterKey) {
      case SpeContactAdvancedFilterKey.IsConfirmed:
        return CommonApiMappingStrategies.getIsConfirmedAdvancedFilter(attributes.operator);
      case SpeContactAdvancedFilterKey.Role:
        return this.getRoleAdvancedFilter(attributes);
      default:
        this.getEnvironmentAdvancedFilter(attributes);
    }
  }

  private getRoleAdvancedFilter(attributes: IAdvancedFilterAttributes): AdvancedFilter {
    const roleCodes = attributes.value[attributes.filterKey];
    const operator = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => ({ roleCode: c });

    return AdvancedFilterTypeMapping.toAdvancedFilter(roleCodes, operator, toFilterCriterion);
  }

  private getEnvironmentAdvancedFilter(attributes: IAdvancedFilterAttributes): AdvancedFilter {
    const toFilterCriterion = criterion => ({ environment: criterion });
    return this.environmentApiMapping.getAdvancedFilter(attributes, toFilterCriterion);
  }

}
