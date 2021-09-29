import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';
import { EnvironmentAdvancedFilterKey } from './environment-advanced-filter-key.enum';

@Injectable()
export class EnvironmentContactFormlyConfiguration {

  public readonly subdomain: FormlyFieldConfig = {
    key: EnvironmentAdvancedFilterKey.Subdomain,
    type: 'input',
    templateOptions: {
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-search',
      placeholder: this.translatePipe.transform('cafe_filters_subdomain'),
    },
  };

  public readonly domain: FormlyFieldConfig = {
    key: EnvironmentAdvancedFilterKey.Domain,
    type: 'environment-domain',
    templateOptions: {
      multiple: true,
      required: true,
      mod: 'palette-grey mod-outlined mod-inline',
      placeholder: this.translatePipe.transform('cafe_filters_domain'),
    },
  };

  constructor(private translatePipe: TranslatePipe) {}
}
