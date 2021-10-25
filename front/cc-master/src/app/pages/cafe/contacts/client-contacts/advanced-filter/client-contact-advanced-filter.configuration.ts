import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import {
  ComparisonOperator,
  IAdvancedFilterConfiguration,
  ICriterionConfiguration,
} from '../../../common/cafe-filters/advanced-filter-form';
import { EnvironmentAdvancedFilterConfiguration } from '../../../environments/advanced-filter';
import { ContactCategory } from '../../common/enums/cafe-contacts-category.enum';
import { ClientContactAdvancedFilterKey } from './client-contact-advanced-filter-key.enum';
import { ClientContactFormlyConfiguration } from './client-contact-formly-configuration.service';

@Injectable()
export class ClientContactAdvancedFilterConfiguration implements IAdvancedFilterConfiguration {
  public readonly categoryId = ContactCategory.Client;
  public readonly criterions: ICriterionConfiguration[] = [
    {
      key: ClientContactAdvancedFilterKey.Environment,
      name: this.translatePipe.transform('cafe_filters_contact_environment'),
      children: this.environmentAdvancedFilterConfiguration.criterions,
    },
    {
      key: ClientContactAdvancedFilterKey.Client,
      name: this.translatePipe.transform('cafe_filters_contact_client'),
      operators: [
        { id: ComparisonOperator.Equals, name: this.translatePipe.transform('cafe_filters_operator_isAmong') },
        { id: ComparisonOperator.NotEquals, name: this.translatePipe.transform('cafe_filters_operator_isNotAmong') },
      ],
      fields: [this.formlyConfiguration.clients],
    },
    {
      key: ClientContactAdvancedFilterKey.IsConfirmed,
      name: this.translatePipe.transform('cafe_filters_contact_isConfirmed'),
      operators: [
        { id: ComparisonOperator.TrueOnly, name: this.translatePipe.transform('cafe_filters_operator_true') },
        { id: ComparisonOperator.FalseOnly, name: this.translatePipe.transform('cafe_filters_operator_false') },
      ],
    },
  ];

  constructor(
    private translatePipe: TranslatePipe,
    private formlyConfiguration: ClientContactFormlyConfiguration,
    private environmentAdvancedFilterConfiguration: EnvironmentAdvancedFilterConfiguration,
  ) {}
}
