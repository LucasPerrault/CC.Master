import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

import { EnvironmentAdvancedFilterKey } from '../../environments/advanced-filter/environment-advanced-filter-key.enum';
import { ICriterionConfiguration } from '../components/advanced-filter-form';

@Injectable()
export class FormlyConfigurationService {

  constructor(private translatePipe: TranslatePipe) {}

  public readonly criterion = (criterions: ICriterionConfiguration[]): FormlyFieldConfig => ({
    key: EnvironmentAdvancedFilterKey.Criterion,
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
