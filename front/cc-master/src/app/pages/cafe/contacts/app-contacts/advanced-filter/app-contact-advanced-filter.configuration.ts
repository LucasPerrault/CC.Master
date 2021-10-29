import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import {
  ComparisonOperator,
  IAdvancedFilterConfiguration,
  ICriterionConfiguration,
} from '../../../common/cafe-filters/advanced-filter-form';
import { EnvironmentAdvancedFilterConfiguration } from '../../../environments/advanced-filter';
import { ContactCategory } from '../../common/enums/cafe-contacts-category.enum';
import { AppContactAdvancedFilterKey } from './app-contact-advanced-filter-key.enum';
import { AppContactCriterionKey } from './app-contact-criterion-key.enum';
import { AppContactFormlyConfiguration } from './app-contact-formly.configuration';

@Injectable()
export class AppContactAdvancedFilterConfiguration implements IAdvancedFilterConfiguration {
  public readonly categoryId = ContactCategory.Application;
  public readonly criterions: ICriterionConfiguration[] = [
    {
      key: AppContactCriterionKey.Environment,
      name: this.translatePipe.transform('cafe_filters_contact_environment'),
      children: this.environmentAdvancedFilterConfiguration.criterions,
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

  constructor(
    private translatePipe: TranslatePipe,
    private formlyConfiguration: AppContactFormlyConfiguration,
    private environmentAdvancedFilterConfiguration: EnvironmentAdvancedFilterConfiguration,
  ) { }
}
