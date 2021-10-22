import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

import { ClientContactAdvancedFilterKey } from './client-contact-advanced-filter-key.enum';

@Injectable()
export class ClientContactFormlyConfiguration {

  public readonly clients: FormlyFieldConfig = {
    key: ClientContactAdvancedFilterKey.Client,
    type: 'api',
    templateOptions: {
      api: '/api/cafe/clients',
      orderBy: 'name,asc',
      multiple: true,
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: 'Sélectionner...',
    },
  };

  constructor(private translatePipe: TranslatePipe) {}
}
