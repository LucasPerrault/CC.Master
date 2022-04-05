import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import {
  ComparisonOperator,
  IAdvancedFilterConfiguration,
  ICriterionConfiguration,
} from '../../../common/components/advanced-filter-form';
import { CriterionFormlyConfigurationService } from '../../../common/services/criterion-formly-configuration.service';
import { EnvironmentAdvancedFilterConfiguration } from '../../../environments/advanced-filter';
import { ClientContactAdvancedFilterKey } from './client-contact-advanced-filter-key.enum';
import { ClientContactCriterionKey } from './client-contact-criterion-key.enum';
import { ClientContactFormlyConfiguration } from './client-contact-formly-configuration.service';

@Injectable()
export class ClientContactAdvancedFilterConfiguration implements IAdvancedFilterConfiguration {
  public readonly criterions: ICriterionConfiguration[] = [
    {
      key: ClientContactCriterionKey.Environment,
      name: this.translatePipe.transform('cafe_filters_contact_environment'),
      children: this.environmentAdvancedFilterConfiguration.criterions,
      childrenFormlyFieldConfigs: [this.commonFormlyConfiguration.criterion(this.environmentAdvancedFilterConfiguration.criterions)],
    },
    {
      key: ClientContactCriterionKey.Client,
      name: this.translatePipe.transform('cafe_filters_contact_client'),
      operators: [
        { id: ComparisonOperator.Equals, name: this.translatePipe.transform('cafe_filters_operator_isAmong') },
        { id: ComparisonOperator.NotEquals, name: this.translatePipe.transform('cafe_filters_operator_isNotAmong') },
      ],
      componentConfigs: () => [
        {
          key: ClientContactAdvancedFilterKey.Client,
          components: [this.formlyConfiguration.clients],
        },
      ],
    },
  ];

  public criterionFormlyFieldConfigs = [
    this.commonFormlyConfiguration.criterion(this.criterions),
  ];

  constructor(
    private translatePipe: TranslatePipe,
    private formlyConfiguration: ClientContactFormlyConfiguration,
    private environmentAdvancedFilterConfiguration: EnvironmentAdvancedFilterConfiguration,
    private commonFormlyConfiguration: CriterionFormlyConfigurationService,
  ) {}
}
