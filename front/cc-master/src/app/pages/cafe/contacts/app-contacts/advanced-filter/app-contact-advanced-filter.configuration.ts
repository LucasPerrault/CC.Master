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
      key: AppContactAdvancedFilterKey.Environment,
      name: this.translatePipe.transform('cafe_filters_environment'),
      children: [
        {
          key: AppContactAdvancedFilterKey.EnvironmentApplications,
          name: this.translatePipe.transform('cafe_filters_environmentApplications'),
          operators: [
              { id: ComparisonOperator.Equals, name: 'contiennent' },
              { id: ComparisonOperator.DoesNotEqual, name: 'ne contiennent pas' },
          ],
          fields: [this.formlyConfiguration.environmentApplications],
        },
      ],
    },
    {
      key: AppContactAdvancedFilterKey.Subdomain,
      name: this.translatePipe.transform('cafe_filters_subdomain'),
      operators: [
        { id: ComparisonOperator.Equals, name: 'égal' },
        { id: ComparisonOperator.DoesNotEqual, name: 'n\'est pas égal ' },
      ],
      fields: [this.formlyConfiguration.subdomain],
    },
    {
      key: AppContactAdvancedFilterKey.Applications,
      name: this.translatePipe.transform('cafe_filters_applications'),
      operators: [
        { id: ComparisonOperator.Equals, name: 'égal' },
        { id: ComparisonOperator.DoesNotEqual, name: 'n\'est pas égal ' },
      ],
      fields: [this.formlyConfiguration.applications],
    },
    {
      key: AppContactAdvancedFilterKey.IsConfirmed,
      name: this.translatePipe.transform('cafe_filters_isConfirmed'),
      operators: [
        { id: ComparisonOperator.TrueOnly, name: 'est vrai' },
        { id: ComparisonOperator.FalseOnly, name: 'est faux' },
        { id: ComparisonOperator.ByPass, name: 'n\'importe pas' },
      ],
    },
  ];

  constructor(
    private translatePipe: TranslatePipe,
    private formlyConfiguration: AppContactFormlyConfiguration,
  ) {}
}
