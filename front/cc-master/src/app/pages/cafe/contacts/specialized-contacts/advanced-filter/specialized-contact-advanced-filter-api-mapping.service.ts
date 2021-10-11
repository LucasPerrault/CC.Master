import { Injectable } from '@angular/core';

import {
  AdvancedFilter, AdvancedFilterFormMapping,
  IAdvancedFilterAttributes,
  IAdvancedFilterForm,
} from '../../../common/cafe-filters/advanced-filter-form';
import { CommonApiMappingStrategies } from '../../common/advanced-filter/common-api-mapping-strategies';
import { SpeContactAdvancedFilterKey } from './specialized-contact-advanced-filter-key.enum';

@Injectable()
export class SpecializedContactAdvancedFilterApiMappingService {
  public toAdvancedFilter(advancedFilterForm: IAdvancedFilterForm): AdvancedFilter {
    return AdvancedFilterFormMapping.toAdvancedFilter(advancedFilterForm, a => this.getAdvancedFilter(a));
  }

  private getAdvancedFilter(attributes: IAdvancedFilterAttributes): AdvancedFilter {
    switch (attributes.filterKey) {
      case SpeContactAdvancedFilterKey.IsConfirmed:
        return CommonApiMappingStrategies.getIsConfirmedAdvancedFilter(attributes.operator);
      case SpeContactAdvancedFilterKey.Subdomain:
        const environments = attributes.value[attributes.filterKey];
        return CommonApiMappingStrategies.getSubdomainAdvancedFilter(attributes.operator, environments);
    }
  }

}
