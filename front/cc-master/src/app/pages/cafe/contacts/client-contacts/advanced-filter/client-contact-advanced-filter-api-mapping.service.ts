import { Injectable } from '@angular/core';

import { IClient } from '../../../billing/models/client.interface';
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
import { ClientContactAdvancedFilterKey } from './client-contact-advanced-filter-key.enum';

@Injectable()
export class ClientContactAdvancedFilterApiMappingService {
  constructor(private environmentApiMapping: EnvironmentAdvancedFilterApiMappingService) {
  }

  public toAdvancedFilter(advancedFilterForm: IAdvancedFilterForm): AdvancedFilter {
    return AdvancedFilterFormMapping.toAdvancedFilter(advancedFilterForm, a => this.getAdvancedFilter(a));
  }

  private getAdvancedFilter(attributes: IAdvancedFilterAttributes): AdvancedFilter {
    switch (attributes.filterKey) {
      case ClientContactAdvancedFilterKey.Client:
        return this.getClientsAdvancedFilter(attributes);
      case ClientContactAdvancedFilterKey.IsConfirmed:
        return CommonApiMappingStrategies.getIsConfirmedAdvancedFilter(attributes.operator);
      default:
        return this.getEnvironmentAdvancedFilter(attributes);
    }
  }

  private getClientsAdvancedFilter(attributes: IAdvancedFilterAttributes): AdvancedFilter {
    const clientIds = attributes.value.fieldValues[attributes.filterKey]?.map((c: IClient) => c.externalId);
    const operator = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => ({ clientId: c });

    return AdvancedFilterTypeMapping.toAdvancedFilter(clientIds, operator, toFilterCriterion);
  }

  private getEnvironmentAdvancedFilter(attributes: IAdvancedFilterAttributes): AdvancedFilter {
    const toFilterCriterion = criterion => ({ environment: criterion });
    return this.environmentApiMapping.getAdvancedFilter(attributes, toFilterCriterion);
  }
}
