import { Injectable } from '@angular/core';
import { IClient } from '@cc/domain/billing/clients';

import {
  AdvancedFilter, AdvancedFilterFormMapping,
  AdvancedFilterTypeMapping,
  ComparisonOperator, IAdvancedFilterAttributes,
  IAdvancedFilterForm,
  LogicalOperator,
} from '../../../common/cafe-filters/advanced-filter-form';
import { CommonApiMappingStrategies } from '../../common/advanced-filter/common-api-mapping-strategies';
import { ClientContactAdvancedFilterKey } from './client-contact-advanced-filter-key.enum';

@Injectable()
export class ClientContactAdvancedFilterApiMappingService {
  public toAdvancedFilter(advancedFilterForm: IAdvancedFilterForm): AdvancedFilter {
    return AdvancedFilterFormMapping.toAdvancedFilter(advancedFilterForm, a => this.getAdvancedFilter(a));
  }

  private getAdvancedFilter(attributes: IAdvancedFilterAttributes): AdvancedFilter {
    switch (attributes.filterKey) {
      case ClientContactAdvancedFilterKey.Clients:
        const clients = attributes.value[attributes.filterKey];
        return this.getClientsAdvancedFilter(attributes.operator, clients);
      case ClientContactAdvancedFilterKey.IsConfirmed:
        return CommonApiMappingStrategies.getIsConfirmedAdvancedFilter(attributes.operator);
      case ClientContactAdvancedFilterKey.Subdomain:
        const environments = attributes.value[attributes.filterKey];
        return CommonApiMappingStrategies.getSubdomainAdvancedFilter(attributes.operator, environments);
    }
  }

  private getClientsAdvancedFilter(operator: ComparisonOperator, clients: IClient[]): AdvancedFilter {
    const criterions = AdvancedFilterTypeMapping.toCriterions(
        operator,
        clients.map(c => c.id),
        c => ({ client: c }),
    );

    return AdvancedFilterTypeMapping.combine(criterions, LogicalOperator.And);
  }

}
