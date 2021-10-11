import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

import { ClientContactAdvancedFilterKey } from './client-contact-advanced-filter-key.enum';

@Injectable()
export class ClientContactFormlyConfiguration {

  public readonly subdomain: FormlyFieldConfig = {
    key: ClientContactAdvancedFilterKey.Subdomain,
    type: 'environment-subdomain',
    templateOptions: {
      multiple: true,
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: 'Sélectionner...',
    },
  };

  public readonly clients: FormlyFieldConfig = {
    key: ClientContactAdvancedFilterKey.Clients,
    type: 'api',
    templateOptions: {
      api: '/api/v3/clients',
      orderBy: 'name,asc',
      multiple: true,
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: 'Sélectionner...',
    },
  };

  public readonly environmentApplications: FormlyFieldConfig = {
    key: ClientContactAdvancedFilterKey.EnvironmentApplications,
    type: 'api',
    templateOptions: {
      api: '/api/cafe/applications',
      standard: 'v4',
      multiple: true,
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: 'Sélectionner...',
    },
  };

  constructor(private translatePipe: TranslatePipe) {}
}
