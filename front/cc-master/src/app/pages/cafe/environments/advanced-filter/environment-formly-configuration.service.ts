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
      placeholder: this.translatePipe.transform('cafe_filters_environment_subdomain_placeholder'),
    },
  };

  public readonly applications: FormlyFieldConfig = {
    key: EnvironmentAdvancedFilterKey.AppInstances,
    type: 'application',
    templateOptions: {
      multiple: true,
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: this.translatePipe.transform('cafe_filters_environment_apps_placeholder'),
    },
  };

  public readonly application: FormlyFieldConfig = {
    key: EnvironmentAdvancedFilterKey.AppInstance,
    type: 'application',
    templateOptions: {
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: this.translatePipe.transform('cafe_filters_environment_app_placeholder'),
    },
  };

  public readonly countries: FormlyFieldConfig = {
    key: EnvironmentAdvancedFilterKey.Countries,
    type: 'country',
    templateOptions: {
      multiple: true,
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: this.translatePipe.transform('cafe_filters_environment_countries_placeholder'),
    },
  };

  public readonly country: FormlyFieldConfig = {
    key: EnvironmentAdvancedFilterKey.Country,
    type: 'country',
    templateOptions: {
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: this.translatePipe.transform('cafe_filters_environment_country_placeholder'),
    },
  };

  public readonly createdAt: FormlyFieldConfig = {
    key: EnvironmentAdvancedFilterKey.CreatedAt,
    type: 'date',
    templateOptions: {
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: this.translatePipe.transform('cafe_filters_environment_createdAt_placeholder'),
    },
  };

  public readonly distributors: FormlyFieldConfig = {
    key: EnvironmentAdvancedFilterKey.Distributors,
    type: 'distributor',
    templateOptions: {
      multiple: true,
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: this.translatePipe.transform('cafe_filters_environment_distributors_placeholder'),
    },
  };

  public readonly distributor: FormlyFieldConfig = {
    key: EnvironmentAdvancedFilterKey.Distributor,
    type: 'distributor',
    templateOptions: {
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: this.translatePipe.transform('cafe_filters_environment_distributor_placeholder'),
    },
  };

  public readonly cluster: FormlyFieldConfig = {
    key: EnvironmentAdvancedFilterKey.Cluster,
    type: 'cluster',
    templateOptions: {
      multiple: true,
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
      placeholder: this.translatePipe.transform('cafe_filters_environment_clusters_placeholder'),
    },
  };

  public readonly distributorType: FormlyFieldConfig = {
    key: EnvironmentAdvancedFilterKey.DistributorType,
    type: 'distributorType',
    templateOptions: {
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer',
    },
  };

  constructor(private translatePipe: TranslatePipe) {}
}
