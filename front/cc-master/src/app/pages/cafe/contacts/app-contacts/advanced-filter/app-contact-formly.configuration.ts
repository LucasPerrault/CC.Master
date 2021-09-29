import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

import { AppContactAdvancedFilterKey } from './app-contact-advanced-filter-key.enum';

@Injectable()
export class AppContactFormlyConfiguration {

  public readonly subdomain: FormlyFieldConfig = {
    key: AppContactAdvancedFilterKey.Subdomain,
    type: 'input',
    templateOptions: {
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-search mod-longer',
      placeholder: this.translatePipe.transform('cafe_filters_subdomain'),
    },
  };

  public readonly applications: FormlyFieldConfig = {
    key: AppContactAdvancedFilterKey.Applications,
    type: 'api',
    templateOptions: {
      api: '/api/v3/products',
      orderBy: 'name,asc',
      multiple: true,
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: this.translatePipe.transform('cafe_filters_applications'),
    },
  };

  public readonly environmentApplications: FormlyFieldConfig = {
    key: AppContactAdvancedFilterKey.EnvironmentApplications,
    type: 'api',
    templateOptions: {
      api: '/api/v3/products',
      orderBy: 'name,asc',
      multiple: true,
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: this.translatePipe.transform('cafe_filters_environmentApplications'),
    },
  };

  constructor(private translatePipe: TranslatePipe) {}
}
