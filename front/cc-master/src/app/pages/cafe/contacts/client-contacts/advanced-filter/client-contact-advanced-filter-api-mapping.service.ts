import { Injectable } from '@angular/core';

import {
  AdvancedFilter,
  AdvancedFilterFormMapping,
  AdvancedFilterTypeMapping,
  cast,
  IAdvancedCriterionAttributes,
  IAdvancedFilterForm,
  IComparisonCriterionAttributes,
} from '../../../common/components/advanced-filter-form';
import { AdvancedFilterOperatorMapping } from '../../../common/components/advanced-filter-form';
import { IClient } from '../../../common/models/client.interface';
import { EnvironmentAdvancedFilterApiMappingService } from '../../../environments/advanced-filter';
import { ClientContactCriterionKey } from './client-contact-criterion-key.enum';

@Injectable()
export class ClientContactAdvancedFilterApiMappingService {
  constructor(private environmentApiMapping: EnvironmentAdvancedFilterApiMappingService) {
  }

  public toAdvancedFilter(advancedFilterForm: IAdvancedFilterForm): AdvancedFilter {
    return AdvancedFilterFormMapping.toAdvancedFilter(advancedFilterForm, a => this.getAdvancedFilter(a));
  }

  private getAdvancedFilter(attributes: IAdvancedCriterionAttributes): AdvancedFilter {
    switch (attributes.criterionKey) {
      case ClientContactCriterionKey.Client:
        return this.getClientsAdvancedFilter(cast<IComparisonCriterionAttributes>(attributes.content));
      case ClientContactCriterionKey.Environment:
        return this.getEnvironmentAdvancedFilter(cast<IAdvancedCriterionAttributes>(attributes.content));
    }
  }

  private getClientsAdvancedFilter(attributes: IComparisonCriterionAttributes): AdvancedFilter {
    const clientIds = attributes.value[attributes.filterKey]?.map((c: IClient) => c.externalId);
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => ({ clientId: c });

    return AdvancedFilterTypeMapping.toAdvancedFilter(clientIds, operator, toFilterCriterion, logicalOperator);
  }

  private getEnvironmentAdvancedFilter(attributes: IAdvancedCriterionAttributes): AdvancedFilter {
    const toFilterCriterion = criterion => ({ environment: criterion });
    return this.environmentApiMapping.getAdvancedFilter(attributes, toFilterCriterion);
  }
}
