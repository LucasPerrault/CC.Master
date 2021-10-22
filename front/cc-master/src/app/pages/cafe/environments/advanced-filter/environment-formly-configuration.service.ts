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

  public readonly cluster: FormlyFieldConfig = {
    key: EnvironmentAdvancedFilterKey.Cluster,
    type: 'api',
    templateOptions: {
      api: '/api/cafe/environments/clusters',
      orderBy: 'name,asc',
      multiple: true,
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: 'Quel beau placeholder de cluster',
    },
  };

  public readonly applications: FormlyFieldConfig = {
    key: EnvironmentAdvancedFilterKey.Applications,
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

  public readonly countries: FormlyFieldConfig = {
    key: EnvironmentAdvancedFilterKey.Countries,
    type: 'country',
    templateOptions: {
      multiple: true,
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: this.translatePipe.transform('cafe_filters_countries'),
    },
  };

  constructor(private translatePipe: TranslatePipe) {}
}
