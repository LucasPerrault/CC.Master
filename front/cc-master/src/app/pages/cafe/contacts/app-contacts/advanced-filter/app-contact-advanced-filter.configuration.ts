import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import {
  ComparisonOperator,
  IAdvancedFilterConfiguration,
  ICriterionConfiguration,
} from '../../../common/components/advanced-filter-form';
import { CriterionFormlyConfigurationService } from '../../../common/services/criterion-formly-configuration.service';
import { EnvironmentAdvancedFilterConfiguration } from '../../../environments/advanced-filter';
import { AppContactAdvancedFilterKey } from './app-contact-advanced-filter-key.enum';
import { AppContactCriterionKey } from './app-contact-criterion-key.enum';
import { AppContactFormlyConfiguration } from './app-contact-formly.configuration';

@Injectable()
export class AppContactAdvancedFilterConfiguration implements IAdvancedFilterConfiguration {
  public readonly criterions: ICriterionConfiguration[] = [
    {
      key: AppContactCriterionKey.Environment,
      name: this.translatePipe.transform('cafe_filters_contact_environment'),
      children: this.environmentAdvancedFilterConfiguration.criterions,
      childrenFormlyFieldConfigs: [this.commonFormlyConfiguration.criterion(this.environmentAdvancedFilterConfiguration.criterions)],
    },
    {
      key: AppContactCriterionKey.AppInstance,
      name: this.translatePipe.transform('cafe_filters_contact_applications'),
      operators: [
        { id: ComparisonOperator.Equals, name: this.translatePipe.transform('cafe_filters_operator_isAmong') },
        { id: ComparisonOperator.NotEquals, name: this.translatePipe.transform('cafe_filters_operator_isNotAmong') },
      ],
      componentConfigs: [
        {
          key: AppContactAdvancedFilterKey.AppInstance,
          components: [this.formlyConfiguration.applications],
        },
      ],
    },
    {
      key: AppContactCriterionKey.IsConfirmed,
      name: this.translatePipe.transform('cafe_filters_contact_isConfirmed'),
      operators: [
        { id: ComparisonOperator.TrueOnly, name: this.translatePipe.transform('cafe_filters_operator_true') },
        { id: ComparisonOperator.FalseOnly, name: this.translatePipe.transform('cafe_filters_operator_false') },
      ],
    },
  ];

  public criterionFormlyFieldConfigs = [
    this.commonFormlyConfiguration.criterion(this.criterions),
  ];

  constructor(
    private translatePipe: TranslatePipe,
    private formlyConfiguration: AppContactFormlyConfiguration,
    private environmentAdvancedFilterConfiguration: EnvironmentAdvancedFilterConfiguration,
    private commonFormlyConfiguration: CriterionFormlyConfigurationService,
  ) { }
}
