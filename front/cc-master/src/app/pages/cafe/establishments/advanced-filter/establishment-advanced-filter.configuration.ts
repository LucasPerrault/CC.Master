import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';

import {
  ComparisonOperator,
  IAdvancedFilterConfiguration,
  ICriterionConfiguration,
} from '../../common/components/advanced-filter-form';
import { FacetScope } from '../../common/models';
import { CriterionFormlyConfigurationService } from '../../common/services/criterion-formly-configuration.service';
import { FacetAdvancedFilterConfigurationService } from '../../common/services/facets';
import { EnvironmentAdvancedFilterConfiguration } from '../../environments/advanced-filter';
import { EnvironmentCriterionKey } from '../../environments/advanced-filter/environment-criterion-key.enum';
import { EstablishmentAdvancedFilterKey } from './establishment-advanced-filter-key.enum';
import { EstablishmentCriterionKey } from './establishment-criterion-key.enum';
import { EstablishmentFormlyConfiguration } from './establishment-formly-configuration.service';

@Injectable()
export class EstablishmentAdvancedFilterConfiguration implements IAdvancedFilterConfiguration {
  public readonly criterions: ICriterionConfiguration[] = [
    {
      key: EstablishmentCriterionKey.Environment,
      name: this.translatePipe.transform('cafe_filters_establishment_environment'),
      children: this.environmentFilterConfiguration.criterions,
      childrenFormlyFieldConfigs: [this.commonFormlyConfiguration.criterion(this.environmentFilterConfiguration.criterions)],
    },
    {
      key: EstablishmentCriterionKey.Country,
      name: this.translatePipe.transform('cafe_filters_establishment_country'),
      operators: [
        { id: ComparisonOperator.Equals, name: this.translatePipe.transform('cafe_filters_operator_isAmong') },
        { id: ComparisonOperator.NotEquals, name: this.translatePipe.transform('cafe_filters_operator_isNotAmong') },
      ],
      componentConfigs: () => [
        {
          key: EstablishmentAdvancedFilterKey.Country,
          components: [this.formlyConfiguration.country],
        },
      ],
    },
    {
      key: EnvironmentCriterionKey.Facets,
      name: this.translatePipe.transform('cafe_filters_environment_facet'),
      children: this.facetFilterConfiguration.criterions(FacetScope.Establishment),
      childrenFormlyFieldConfigs: this.facetFilterConfiguration.criterionFormlyFieldConfigs(FacetScope.Establishment),
    },
  ];

  public criterionFormlyFieldConfigs = [
    this.commonFormlyConfiguration.criterion(this.criterions),
  ];

  constructor(
    private translatePipe: TranslatePipe,
    private formlyConfiguration: EstablishmentFormlyConfiguration,
    private environmentFilterConfiguration: EnvironmentAdvancedFilterConfiguration,
    private commonFormlyConfiguration: CriterionFormlyConfigurationService,
    private facetFilterConfiguration: FacetAdvancedFilterConfigurationService,
  ) {}
}
