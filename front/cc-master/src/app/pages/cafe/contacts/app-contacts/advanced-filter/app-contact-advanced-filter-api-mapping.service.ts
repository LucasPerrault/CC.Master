import { Injectable } from '@angular/core';

import {
  AdvancedFilter,
  AdvancedFilterFormMapping,
  AdvancedFilterTypeMapping,
  IAdvancedFilterAttributes,
  IAdvancedFilterForm,
  IComparisonFilterCriterionEncapsulation,
} from '../../../common/cafe-filters/advanced-filter-form';
import { EnvironmentAdvancedFilterApiMappingService } from '../../../environments/advanced-filter';
import { IAppInstance } from '../../../environments/models/app-instance.interface';
import { CommonApiMappingStrategies } from '../../common/advanced-filter/common-api-mapping-strategies';
import { AppContactAdvancedFilterKey } from './app-contact-advanced-filter-key.enum';

@Injectable()
export class AppContactAdvancedFilterApiMappingService {
  constructor(private environmentApiMapping: EnvironmentAdvancedFilterApiMappingService) {
  }

  public toAdvancedFilter(advancedFilterForm: IAdvancedFilterForm): AdvancedFilter {
    return AdvancedFilterFormMapping.toAdvancedFilter(advancedFilterForm, a => this.getAdvancedFilter(a));
  }

  private getAdvancedFilter(attributes: IAdvancedFilterAttributes): AdvancedFilter {
    switch (attributes.filterKey) {
      case AppContactAdvancedFilterKey.AppInstance:
        return this.getAppInstanceAdvancedFilter(attributes, c => ({ appInstance: { applicationId: c } }));
      case AppContactAdvancedFilterKey.IsConfirmed:
        return CommonApiMappingStrategies.getIsConfirmedAdvancedFilter(attributes.operator);
      default:
        return this.environmentApiMapping.getAdvancedFilter(attributes, criterion => ({ environment: criterion }));
    }
  }

  private getAppInstanceAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    toIFilterCriterion: IComparisonFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const appInstanceIds = attributes.value[attributes.filterKey]?.map((a: IAppInstance) => a.id);
    return AdvancedFilterTypeMapping.toAdvancedFilter(appInstanceIds, attributes.operator, toIFilterCriterion);
  }
}
