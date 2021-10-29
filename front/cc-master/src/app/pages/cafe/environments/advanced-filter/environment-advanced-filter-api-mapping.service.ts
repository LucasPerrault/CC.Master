import { Injectable } from '@angular/core';
import { IDistributor } from '@cc/domain/billing/distributors';
import { IEnvironment } from '@cc/domain/environments';

import {
  AdvancedFilter,
  AdvancedFilterFormMapping,
  AdvancedFilterTypeMapping,
  defaultEncapsulation,
  IAdvancedFilterAttributes,
  IAdvancedFilterForm,
  IFilterCriterionEncapsulation,
} from '../../common/cafe-filters/advanced-filter-form';
import { AdvancedFilterOperatorMapping } from '../../common/cafe-filters/advanced-filter-form';
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
        return this.getAppInstancesAdvancedFilter(attributes, encapsulate);
      case EnvironmentAdvancedFilterKey.AppInstance:
        return this.getAppInstanceAdvancedFilter(attributes, encapsulate);
      case EnvironmentAdvancedFilterKey.Countries:
        return this.getCountriesAdvancedFilter(attributes, encapsulate);
      case EnvironmentAdvancedFilterKey.Country:
        return this.getCountryAdvancedFilter(attributes, encapsulate);
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
    const subdomains = attributes.value.fieldValues[attributes.filterKey].map((e: IEnvironment) => e.subDomain);
    const operator = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => (encapsulate({ subdomain: c }));

    return AdvancedFilterTypeMapping.toAdvancedFilter(subdomains, operator, toFilterCriterion);
  }

  private getAppInstancesAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const applicationIds = attributes.value.fieldValues[attributes.filterKey].map((a: IApplication) => a.id);
    const { operator, itemsMatched, logicalOperator } = AdvancedFilterOperatorMapping.getListComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = (c) => encapsulate({
      appInstances: { applicationId: c, itemsMatched },
    });

    return AdvancedFilterTypeMapping.toAdvancedFilter(applicationIds, operator, toFilterCriterion, logicalOperator);
  }

  private getAppInstanceAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const applicationId = attributes.value.fieldValues[attributes.filterKey].id;
    const { operator, itemsMatched, logicalOperator } = AdvancedFilterOperatorMapping.getListComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = (c) => encapsulate({
      appInstances: { applicationId: c, itemsMatched },
    });

    return AdvancedFilterTypeMapping.toAdvancedFilter([applicationId], operator, toFilterCriterion, logicalOperator);
  }

  private getCountriesAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const countryIds = attributes.value.fieldValues[attributes.filterKey].map((c: ICountry) => c.id);
    const { operator, itemsMatched, logicalOperator } = AdvancedFilterOperatorMapping.getListComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => (encapsulate({
      legalUnits: { countryId: c, itemsMatched },
    }));

    return AdvancedFilterTypeMapping.toAdvancedFilter(countryIds, operator, toFilterCriterion, logicalOperator);
  }

  private getCountryAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const countryId = attributes.value.fieldValues[attributes.filterKey].id;
    const { operator, itemsMatched, logicalOperator } = AdvancedFilterOperatorMapping.getListComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => (encapsulate({
      legalUnits: { countryId: c, itemsMatched },
    }));

    return AdvancedFilterTypeMapping.toAdvancedFilter([countryId], operator, toFilterCriterion, logicalOperator);
  }

  private getCreatedAtAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const createdAt = attributes.value.fieldValues[attributes.filterKey];
    const operator = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => (encapsulate({ createdAt: c }));

    return AdvancedFilterTypeMapping.toAdvancedFilter([createdAt], operator, toFilterCriterion);
  }

  private getDistributorsAdvancedFilter(attributes: IAdvancedFilterAttributes, encapsulate: IFilterCriterionEncapsulation): AdvancedFilter {
    const distributorIds = attributes.value.fieldValues[attributes.filterKey]?.map((d: IDistributor) => d.id);
    const { operator, itemsMatched, logicalOperator } = AdvancedFilterOperatorMapping.getListComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => (encapsulate({
      distributorId: c,
      itemsMatched,
    }));

    return AdvancedFilterTypeMapping.toAdvancedFilter(distributorIds, operator, toFilterCriterion, logicalOperator);
  }
}
