import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

import { SpeContactAdvancedFilterKey } from './specialized-contact-advanced-filter-key.enum';

@Injectable()
export class SpecializedContactFormlyConfiguration {

  public readonly subdomain: FormlyFieldConfig = {
    key: SpeContactAdvancedFilterKey.Subdomain,
    type: 'input',
    templateOptions: {
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-search mod-longer',
      placeholder: this.translatePipe.transform('cafe_filters_subdomain'),
    },
  };

  constructor(private translatePipe: TranslatePipe) {}
}
