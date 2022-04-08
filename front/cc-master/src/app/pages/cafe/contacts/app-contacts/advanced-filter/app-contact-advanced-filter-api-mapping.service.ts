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
import { EnvironmentAdvancedFilterApiMappingService } from '../../../environments/advanced-filter';
import { IAppInstance } from '../../../environments/models/app-instance.interface';
import { AppContactCriterionKey } from './app-contact-criterion-key.enum';

@Injectable()
export class AppContactAdvancedFilterApiMappingService {
  constructor(private environmentApiMapping: EnvironmentAdvancedFilterApiMappingService) {
  }

  public toAdvancedFilter(advancedFilterForm: IAdvancedFilterForm): AdvancedFilter {
    return AdvancedFilterFormMapping.toAdvancedFilter(advancedFilterForm, a => this.getAdvancedFilter(a));
  }

  private getAdvancedFilter(attributes: IAdvancedCriterionAttributes): AdvancedFilter {
    switch (attributes.criterionKey) {
      case AppContactCriterionKey.AppInstance:
        return this.getAppInstanceAdvancedFilter(cast<IComparisonCriterionAttributes>(attributes.content));
      case AppContactCriterionKey.IsConfirmed:
        return this.getIsConfirmedAdvancedFilter(cast<IComparisonCriterionAttributes>(attributes.content));
      case AppContactCriterionKey.Environment:
        return this.getEnvironmentAdvancedFilter(cast<IAdvancedCriterionAttributes>(attributes.content));
    }
  }

  private getAppInstanceAdvancedFilter(attributes: IComparisonCriterionAttributes): AdvancedFilter {
    const appInstanceIds = attributes.value[attributes.filterKey]?.map((a: IAppInstance) => a.id);
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => ({ appInstance: { applicationId: c } });

    return AdvancedFilterTypeMapping.toAdvancedFilter(appInstanceIds, operator, toFilterCriterion, logicalOperator);
  }

  private getIsConfirmedAdvancedFilter(attributes: IComparisonCriterionAttributes): AdvancedFilter {
    const isConfirmed = attributes.value[attributes.filterKey] as boolean;
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => ({ isConfirmed: c });

    return AdvancedFilterTypeMapping.toAdvancedFilter([isConfirmed], operator, toFilterCriterion, logicalOperator);
  }

  private getEnvironmentAdvancedFilter(attributes: IAdvancedCriterionAttributes): AdvancedFilter {
    const toFilterCriterion = criterion => ({ environment: criterion });
    return this.environmentApiMapping.getAdvancedFilter(attributes, toFilterCriterion);
  }
}
