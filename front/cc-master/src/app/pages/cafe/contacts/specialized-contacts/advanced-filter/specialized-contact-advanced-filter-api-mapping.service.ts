import { Injectable } from '@angular/core';

import {
  AdvancedFilter, AdvancedFilterFormMapping, AdvancedFilterTypeMapping,
  IAdvancedFilterAttributes,
  IAdvancedFilterForm, IComparisonFilterCriterionEncapsulation,
} from '../../../common/cafe-filters/advanced-filter-form';
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
        return this.getRoleAdvancedFilter(attributes, c => ({ roleCode: c }));
      default:
        return this.environmentApiMapping.getAdvancedFilter(attributes, criterion => ({ environment: criterion }));
    }
  }

  private getRoleAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    toIFilterCriterion: IComparisonFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const roleCodes = attributes.value[attributes.filterKey];
    return AdvancedFilterTypeMapping.toAdvancedFilter(roleCodes, attributes.operator, toIFilterCriterion);
  }

}
