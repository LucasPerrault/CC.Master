import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import {
  ComparisonOperator,
  IAdvancedFilterConfiguration,
  ICriterionConfiguration,
} from '../../../common/cafe-filters/advanced-filter-form';
import { AppContactAdvancedFilterKey } from '../../app-contacts/advanced-filter/app-contact-advanced-filter-key.enum';
import { ContactCategory } from '../../common/enums/cafe-contacts-category.enum';
import { ClientContactAdvancedFilterKey } from './client-contact-advanced-filter-key.enum';
import { ClientContactFormlyConfiguration } from './client-contact-formly-configuration.service';

@Injectable()
export class ClientContactAdvancedFilterConfiguration implements IAdvancedFilterConfiguration {
  public readonly categoryId = ContactCategory.Client;
  public readonly criterions: ICriterionConfiguration[] = [
    {
      key: AppContactAdvancedFilterKey.Environment,
      name: 'Dans l\'environnement',
      children: [
        {
          key: AppContactAdvancedFilterKey.EnvironmentApplications,
          name: 'Les applications',
          operators: [
            { id: ComparisonOperator.Equals, name: 'contiennent' },
            { id: ComparisonOperator.DoesNotEqual, name: 'ne contiennent pas' },
          ],
          fields: [this.formlyConfiguration.environmentApplications],
        },
        {
          key: AppContactAdvancedFilterKey.Subdomain,
          name: 'les sous-domaine',
          operators: [
            { id: ComparisonOperator.Equals, name: 'est parmi' },
            { id: ComparisonOperator.DoesNotEqual, name: 'n\'est pas parmi' },
          ],
          fields: [this.formlyConfiguration.subdomain],
        },
      ],
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
  ) {}
}
