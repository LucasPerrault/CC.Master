import { Injectable } from '@angular/core';
import { IDistributor } from '@cc/domain/billing/distributors';
import { IEnvironment } from '@cc/domain/environments';

import {
  AdvancedFilter,
  AdvancedFilterFormMapping,
  AdvancedFilterTypeMapping,
  ComparisonOperator,
  defaultEncapsulation,
  IAdvancedFilterAttributes,
  IAdvancedFilterForm,
  IFilterCriterionEncapsulation,
} from '../../common/cafe-filters/advanced-filter-form';
import { IApplication } from '../models/app-instance.interface';
import { ICountry } from '../models/legal-unit.interface';
import { EnvironmentAdvancedFilterKey } from './environment-advanced-filter-key.enum';

@Injectable()
export class EnvironmentAdvancedFilterApiMappingService {
  public toAdvancedFilter(advancedFilterForm: IAdvancedFilterForm): AdvancedFilter {
    return AdvancedFilterFormMapping.toAdvancedFilter(advancedFilterForm, a => this.getAdvancedFilter(a));
  }

  public getAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation = defaultEncapsulation,
  ): AdvancedFilter {
    switch (attributes.filterKey) {
      case EnvironmentAdvancedFilterKey.Subdomain:
        return this.getSubdomainAdvancedFilter(attributes, encapsulate);
      case EnvironmentAdvancedFilterKey.AppInstances:
        return this.getAppInstanceAdvancedFilter(attributes, encapsulate);
      case EnvironmentAdvancedFilterKey.Countries:
        return this.getCountriesAdvancedFilter(attributes, encapsulate);
      case EnvironmentAdvancedFilterKey.CreatedAt:
        return this.getCreatedAtAdvancedFilter(attributes, encapsulate);
      case EnvironmentAdvancedFilterKey.Distributors:
        return this.getDistributorsAdvancedFilter(attributes, encapsulate);
    }
  }

  private getSubdomainAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const subdomains = attributes.value[attributes.filterKey].map((e: IEnvironment) => e.subDomain);
    return AdvancedFilterTypeMapping.toAdvancedFilter(
      subdomains,
      attributes.operator,
      c => (encapsulate({ subdomain: c })),
    );
  }

  private getAppInstanceAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const applicationIds = attributes.value[attributes.filterKey].map((a: IApplication) => a.id);
    if (attributes.operator === ComparisonOperator.Contains) {
      return AdvancedFilterTypeMapping.toAdvancedFilterWithContainsOperator(
        applicationIds,
        c => (encapsulate({ appInstances: { applicationId: c } })),
      );
    }

    return AdvancedFilterTypeMapping.toAdvancedFilter(
      applicationIds,
      attributes.operator,
      c => (encapsulate({ appInstances: { applicationId: c } })),
    );
  }

  private getCountriesAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const countryIds = attributes.value[attributes.filterKey].map((c: ICountry) => c.id);
    if (attributes.operator === ComparisonOperator.Contains) {
      return AdvancedFilterTypeMapping.toAdvancedFilterWithContainsOperator(
        countryIds,
        c => (encapsulate({ legalUnits: { countryId: c } })),
      );
    }

    return AdvancedFilterTypeMapping.toAdvancedFilter(
      countryIds,
      attributes.operator,
      c => (encapsulate({ legalUnits: { countryId: c } })),
    );
  }

  private getCreatedAtAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const createdAt = attributes.value[attributes.filterKey];
    return AdvancedFilterTypeMapping.toAdvancedFilter(
      [createdAt],
      attributes.operator,
      c => (encapsulate({ createdAt: c })),
    );
  }

  private getDistributorsAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation): AdvancedFilter {
    const distributorIds = attributes.value[attributes.filterKey]?.map((d: IDistributor) => d.id);
    if (attributes.operator === ComparisonOperator.Contains) {
      return AdvancedFilterTypeMapping.toAdvancedFilterWithContainsOperator(
        distributorIds,
        c => (encapsulate({ distributorId: c })),
      );
    }

    return AdvancedFilterTypeMapping.toAdvancedFilter(
      distributorIds,
      attributes.operator,
      c => (encapsulate({ distributorId: c })),
    );
  }
}
