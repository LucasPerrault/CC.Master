import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import {
  ComparisonOperator,
  IAdvancedFilterConfiguration,
  ICriterionConfiguration,
} from '../../../common/cafe-filters/advanced-filter-form';
import { EnvironmentAdvancedFilterConfiguration } from '../../../environments/advanced-filter';
import { ContactCategory } from '../../common/enums/cafe-contacts-category.enum';
import { SpeContactAdvancedFilterKey } from './specialized-contact-advanced-filter-key.enum';
import { SpeContactCriterionKey } from './specialized-contact-criterion-key.enum';
import { SpecializedContactFormlyConfiguration } from './specialized-contact-formly.configuration';

@Injectable()
export class SpecializedContactAdvancedFilterConfiguration implements IAdvancedFilterConfiguration {
  public readonly categoryId = ContactCategory.Specialized;
  public readonly criterions: ICriterionConfiguration[] = [
    {
      key: SpeContactCriterionKey.Environment,
      name: this.translatePipe.transform('cafe_filters_contact_environment'),
      children: this.environmentAdvancedFilterConfiguration.criterions,
    },
    {
      key: SpeContactCriterionKey.Role,
      name: this.translatePipe.transform('cafe_filters_contact_role'),
      operators: [
        { id: ComparisonOperator.Equals, name: this.translatePipe.transform('cafe_filters_operator_isAmong') },
        { id: ComparisonOperator.NotEquals, name: this.translatePipe.transform('cafe_filters_operator_isNotAmong') },
      ],
      fields: [this.formlyConfiguration.roles],
      componentConfigs: [
        {
          key: SpeContactAdvancedFilterKey.Role,
          components: [this.formlyConfiguration.roles],
        },
      ],
    },
    {
      key: SpeContactCriterionKey.IsConfirmed,
      name: this.translatePipe.transform('cafe_filters_contact_isConfirmed'),
      operators: [
        { id: ComparisonOperator.TrueOnly, name: this.translatePipe.transform('cafe_filters_operator_true') },
        { id: ComparisonOperator.FalseOnly, name: this.translatePipe.transform('cafe_filters_operator_false') },
      ],
    },
  ];

  constructor(
    private translatePipe: TranslatePipe,
    private environmentAdvancedFilterConfiguration: EnvironmentAdvancedFilterConfiguration,
    private formlyConfiguration: SpecializedContactFormlyConfiguration,
  ) {}
}
