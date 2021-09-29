import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import {
  ComparisonOperator,
  IAdvancedFilterConfiguration,
  ICriterionConfiguration,
} from '../../../common/cafe-filters/advanced-filter-form';
import { ContactCategory } from '../../common/enums/cafe-contacts-category.enum';
import { ClientContactAdvancedFilterKey } from './client-contact-advanced-filter-key.enum';
import { ClientContactFormlyConfiguration } from './client-contact-formly-configuration.service';

@Injectable()
export class ClientContactAdvancedFilterConfiguration implements IAdvancedFilterConfiguration {
  public readonly categoryId = ContactCategory.Client;
  public readonly criterions: ICriterionConfiguration[] = [
    {
      key: ClientContactAdvancedFilterKey.Subdomain,
      name: this.translatePipe.transform('cafe_filters_subdomain'),
      operators: [ComparisonOperator.Equals, ComparisonOperator.DoesNotEqual],
      fields: [this.formlyConfiguration.subdomain],
    },
    {
      key: ClientContactAdvancedFilterKey.Clients,
      name: this.translatePipe.transform('cafe_filters_client'),
      operators: [ComparisonOperator.Equals, ComparisonOperator.DoesNotEqual],
      fields: [this.formlyConfiguration.clients],
    },
    {
      key: ClientContactAdvancedFilterKey.IsConfirmed,
      name: this.translatePipe.transform('cafe_filters_isConfirmed'),
      operators: [ComparisonOperator.TrueOnly, ComparisonOperator.FalseOnly, ComparisonOperator.ByPass],
    },
  ];

  constructor(
    private translatePipe: TranslatePipe,
    private formlyConfiguration: ClientContactFormlyConfiguration,
  ) {}
}
