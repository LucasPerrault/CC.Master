import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

import { SpeContactAdvancedFilterKey } from './specialized-contact-advanced-filter-key.enum';

@Injectable()
export class SpecializedContactFormlyConfiguration {

  public readonly roles: FormlyFieldConfig = {
    key: SpeContactAdvancedFilterKey.Role,
    type: 'specialized-contact-role',
    templateOptions: {
      multiple: true,
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: this.translatePipe.transform('cafe_filters_contact_role_placeholder'),
    },
  };

  constructor(private translatePipe: TranslatePipe) {}
}
