import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import {
  ComparisonOperator,
  IAdvancedFilterConfiguration,
  ICriterionConfiguration,
} from '../../../common/cafe-filters/advanced-filter-form';
import { ContactCategory } from '../../common/enums/cafe-contacts-category.enum';
import { SpeContactAdvancedFilterKey } from './specialized-contact-advanced-filter-key.enum';
import { SpecializedContactFormlyConfiguration } from './specialized-contact-formly-configuration.service';

@Injectable()
export class SpecializedContactAdvancedFilterConfiguration implements IAdvancedFilterConfiguration {
  public readonly categoryId = ContactCategory.Specialized;
  public readonly criterions: ICriterionConfiguration[] = [
    {
      key: SpeContactAdvancedFilterKey.Subdomain,
      name: this.translatePipe.transform('cafe_filters_subdomain'),
      operators: [
        { id: ComparisonOperator.Equals, name: 'égal' },
        { id: ComparisonOperator.DoesNotEqual, name: 'n\'est pas égal ' },
      ],
      fields: [this.formlyConfiguration.subdomain],
    },
    {
      key: SpeContactAdvancedFilterKey.IsConfirmed,
      name: this.translatePipe.transform('cafe_filters_isConfirmed'),
      operators: [
        { id: ComparisonOperator.TrueOnly, name: 'est vrai' },
        { id: ComparisonOperator.FalseOnly, name: 'est faux' },
      ],
    },
  ];

  constructor(
    private translatePipe: TranslatePipe,
    private formlyConfiguration: SpecializedContactFormlyConfiguration,
  ) {}
}
