import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

import { ClientContactAdvancedFilterKey } from './client-contact-advanced-filter-key.enum';

@Injectable()
export class ClientContactFormlyConfiguration {

  public readonly subdomain: FormlyFieldConfig = {
    key: ClientContactAdvancedFilterKey.Subdomain,
    type: 'input',
    templateOptions: {
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-search',
      placeholder: this.translatePipe.transform('cafe_filters_subdomain'),
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
      mod: 'palette-grey mod-outlined mod-inline',
      placeholder: this.translatePipe.transform('cafe_filters_client'),
    },
  };

  constructor(private translatePipe: TranslatePipe) {}
}
