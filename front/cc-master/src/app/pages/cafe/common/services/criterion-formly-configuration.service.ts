import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

import { ICriterionConfiguration } from '../components/advanced-filter-form';

export enum AdvancedFilterKey {
  Criterion = 'criterion',
}

@Injectable()
export class CriterionFormlyConfigurationService {

  constructor(private translatePipe: TranslatePipe) {}

  public readonly criterion = (criterions: ICriterionConfiguration[]): FormlyFieldConfig => ({
    key: AdvancedFilterKey.Criterion,
    type: 'criterion',
    templateOptions: {
      multiple: true,
      required: true,
      mod: 'palette-grey mod-outlined mod-inline is-required',
      options: criterions,
      placeholder: this.translatePipe.transform('cafe_filters_environment_criterion_placeholder'),
    },
  });
}
