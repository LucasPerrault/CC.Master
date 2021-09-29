import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import {
  ComparisonOperator,
  IAdvancedFilterConfiguration,
  ICriterionConfiguration,
} from '../../../common/cafe-filters/advanced-filter-form';
import { ContactCategory } from '../../common/enums/cafe-contacts-category.enum';
import { AppContactAdvancedFilterKey } from './app-contact-advanced-filter-key.enum';
import { AppContactFormlyConfiguration } from './app-contact-formly.configuration';

@Injectable()
export class AppContactAdvancedFilterConfiguration implements IAdvancedFilterConfiguration {
  public readonly categoryId = ContactCategory.Application;
  public readonly criterions: ICriterionConfiguration[] = [
    {
      key: AppContactAdvancedFilterKey.Subdomain,
      name: this.translatePipe.transform('cafe_filters_subdomain'),
      operators: [ComparisonOperator.Equals, ComparisonOperator.DoesNotEqual],
      fields: [this.formlyConfiguration.subdomain],
    },
    {
      key: AppContactAdvancedFilterKey.Application,
      name: this.translatePipe.transform('cafe_filters_application'),
      operators: [ComparisonOperator.Equals, ComparisonOperator.DoesNotEqual],
      fields: [this.formlyConfiguration.application],
    },
    {
      key: AppContactAdvancedFilterKey.IsConfirmed,
      name: this.translatePipe.transform('cafe_filters_isConfirmed'),
      operators: [ComparisonOperator.TrueOnly, ComparisonOperator.FalseOnly, ComparisonOperator.ByPass],
    },
  ];

  constructor(
    private translatePipe: TranslatePipe,
    private formlyConfiguration: AppContactFormlyConfiguration,
  ) {}
}
