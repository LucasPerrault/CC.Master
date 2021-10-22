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
import { AppContactFormlyConfiguration } from './app-contact-formly.configuration';

@Injectable()
export class AppContactAdvancedFilterConfiguration implements IAdvancedFilterConfiguration {
  public readonly categoryId = ContactCategory.Application;
  public readonly criterions: ICriterionConfiguration[] = [
    {
      key: AppContactAdvancedFilterKey.Environment,
      name: this.translatePipe.transform('cafe_filters_environment'),
      children: this.environmentAdvancedFilterConfiguration.criterions,
    },
    {
      key: AppContactAdvancedFilterKey.Applications,
      name: 'L\'application gérée',
      operators: [
        { id: ComparisonOperator.Equals, name: 'est parmi' },
        { id: ComparisonOperator.DoesNotEqual, name: 'n\'est pas parmi' },
      ],
      fields: [this.formlyConfiguration.applications],
    },
    {
      key: AppContactAdvancedFilterKey.IsConfirmed,
      name: 'Est confirmé ?',
      operators: [
        { id: ComparisonOperator.TrueOnly, name: 'Oui' },
        { id: ComparisonOperator.FalseOnly, name: 'Non' },
      ],
    },
  ];

  constructor(
    private translatePipe: TranslatePipe,
    private formlyConfiguration: AppContactFormlyConfiguration,
    private environmentAdvancedFilterConfiguration: EnvironmentAdvancedFilterConfiguration,
  ) { }
}
