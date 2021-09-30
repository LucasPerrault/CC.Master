import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

import { EnvironmentAdvancedFilterKey } from './environment-advanced-filter-key.enum';

@Injectable()
export class EnvironmentFormlyConfiguration {

  public readonly subdomain: FormlyFieldConfig = {
    key: EnvironmentAdvancedFilterKey.Subdomain,
    type: 'environment-subdomain',
    templateOptions: {
      multiple: true,
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: this.translatePipe.transform('cafe_filters_subdomain'),
    },
  };

  public readonly domain: FormlyFieldConfig = {
    key: EnvironmentAdvancedFilterKey.Domain,
    type: 'environment-domain',
    templateOptions: {
      multiple: true,
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: this.translatePipe.transform('cafe_filters_domain'),
    },
  };

  constructor(private translatePipe: TranslatePipe) {}
}
