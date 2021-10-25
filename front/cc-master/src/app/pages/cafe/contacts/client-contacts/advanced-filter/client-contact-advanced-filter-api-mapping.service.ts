import { Injectable } from '@angular/core';

import { IClient } from '../../../billing/models/client.interface';
import {
  AdvancedFilter,
  AdvancedFilterFormMapping,
  AdvancedFilterTypeMapping,
  IAdvancedFilterAttributes,
  IAdvancedFilterForm,
  IComparisonFilterCriterionEncapsulation,
} from '../../../common/cafe-filters/advanced-filter-form';
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
        return this.getClientsAdvancedFilter(attributes, c => ({ clientId: c }));
      case ClientContactAdvancedFilterKey.IsConfirmed:
        return CommonApiMappingStrategies.getIsConfirmedAdvancedFilter(attributes.operator);
      default:
        return this.environmentApiMapping.getAdvancedFilter(attributes, criterion => ({ environment: criterion }));
    }
  }

  private getClientsAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    toIFilterCriterion: IComparisonFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const clientIds = attributes.value[attributes.filterKey]?.map((c: IClient) => c.externalId);
    return AdvancedFilterTypeMapping.toAdvancedFilter(clientIds, attributes.operator, toIFilterCriterion);
  }
}
