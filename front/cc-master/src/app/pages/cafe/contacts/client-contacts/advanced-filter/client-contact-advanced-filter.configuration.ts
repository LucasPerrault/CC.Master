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
      name: 'Dans l\'environnement',
      children: this.environmentAdvancedFilterConfiguration.criterions,
    },
    {
      key: ClientContactAdvancedFilterKey.Clients,
      name: 'le client',
      operators: [
        { id: ComparisonOperator.Equals, name: 'est parmi' },
        { id: ComparisonOperator.DoesNotEqual, name: 'n\'est pas parmi' },
      ],
      fields: [this.formlyConfiguration.clients],
    },
    {
      key: ClientContactAdvancedFilterKey.IsConfirmed,
      name: 'Est confirmé ?',
      operators: [
        { id: ComparisonOperator.TrueOnly, name: 'Oui' },
        { id: ComparisonOperator.FalseOnly, name: 'Non' },
      ],
    },
  ];

  constructor(
    private translatePipe: TranslatePipe,
    private formlyConfiguration: ClientContactFormlyConfiguration,
    private environmentAdvancedFilterConfiguration: EnvironmentAdvancedFilterConfiguration,
  ) {}
}
