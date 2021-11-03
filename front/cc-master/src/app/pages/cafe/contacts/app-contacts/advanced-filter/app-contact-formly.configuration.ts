import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

import { AppContactAdvancedFilterKey } from './app-contact-advanced-filter-key.enum';

@Injectable()
export class AppContactFormlyConfiguration {

  public readonly applications: FormlyFieldConfig = {
    key: AppContactAdvancedFilterKey.AppInstance,
    type: 'api',
    templateOptions: {
      api: '/api/cafe/applications',
      standard: 'v4',
      multiple: true,
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: this.translatePipe.transform('cafe_filters_contact_appInstances_placeholder'),
    },
  };

  constructor(private translatePipe: TranslatePipe) {}
}
