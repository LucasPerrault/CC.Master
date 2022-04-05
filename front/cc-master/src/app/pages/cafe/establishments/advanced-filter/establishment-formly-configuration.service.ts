import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

import { EstablishmentCriterionKey } from './establishment-criterion-key.enum';

@Injectable()
export class EstablishmentFormlyConfiguration {

  public readonly country: FormlyFieldConfig = {
    key: EstablishmentCriterionKey.Country,
    type: 'country',
    templateOptions: {
      multiple: true,
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: this.translatePipe.transform('cafe_filters_establishment_country'),
    },
  };

  constructor(private translatePipe: TranslatePipe) {}
}
