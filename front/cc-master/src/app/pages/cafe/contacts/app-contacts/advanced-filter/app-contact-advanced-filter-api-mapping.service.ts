import { Injectable } from '@angular/core';

import {
  AdvancedFilter, AdvancedFilterFormMapping,
  AdvancedFilterTypeMapping,
  ComparisonOperator,
  IAdvancedFilterAttributes,
  IAdvancedFilterForm,
  LogicalOperator,
} from '../../../common/cafe-filters/advanced-filter-form';
import { IAppInstance } from '../../../environments/models/app-instance.interface';
import { CommonApiMappingStrategies } from '../../common/advanced-filter/common-api-mapping-strategies';
import { AppContactAdvancedFilterKey } from './app-contact-advanced-filter-key.enum';

@Injectable()
export class AppContactAdvancedFilterApiMappingService {
  public toAdvancedFilter(advancedFilterForm: IAdvancedFilterForm): AdvancedFilter {
    return AdvancedFilterFormMapping.toAdvancedFilter(advancedFilterForm, a => this.getAdvancedFilter(a));
  }

  private getAdvancedFilter(attributes: IAdvancedFilterAttributes): AdvancedFilter {
    switch (attributes.filterKey) {
      case AppContactAdvancedFilterKey.EnvironmentApplications:
        const envAppInstances = attributes.value[attributes.filterKey];
        return CommonApiMappingStrategies.getEnvironmentAppInstancesAdvancedFilter(attributes.operator, envAppInstances);
      case AppContactAdvancedFilterKey.Applications:
        const appInstances = attributes.value[attributes.filterKey];
        return this.getAppInstanceAdvancedFilter(attributes.operator, appInstances);
      case AppContactAdvancedFilterKey.IsConfirmed:
        return CommonApiMappingStrategies.getIsConfirmedAdvancedFilter(attributes.operator);
      case AppContactAdvancedFilterKey.Subdomain:
        const environments = attributes.value[attributes.filterKey];
        return CommonApiMappingStrategies.getSubdomainAdvancedFilter(attributes.operator, environments);
    }
  }

  private getAppInstanceAdvancedFilter(operator: ComparisonOperator, appInstances: IAppInstance[]): AdvancedFilter {
    const criterions = AdvancedFilterTypeMapping.toCriterions(
        operator,
        appInstances.map(a => a.id),
        c => ({ appInstance: { applicationId: c } }),
    );
    return AdvancedFilterTypeMapping.combine(criterions, LogicalOperator.Or);
  }

}
