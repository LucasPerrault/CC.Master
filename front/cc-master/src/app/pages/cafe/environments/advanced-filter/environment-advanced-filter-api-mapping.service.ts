import { Injectable } from '@angular/core';
import { IDistributor } from '@cc/domain/billing/distributors';
import { IEnvironment } from '@cc/domain/environments';

import {
  AdvancedFilter,
  AdvancedFilterFormMapping,
  AdvancedFilterTypeMapping, ComparisonOperator,
  defaultEncapsulation,
  IAdvancedFilterAttributes,
  IAdvancedFilterForm,
  IFilterCriterionEncapsulation,
} from '../../common/cafe-filters/advanced-filter-form';
import { AdvancedFilterOperatorMapping } from '../../common/cafe-filters/advanced-filter-form';
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
        const appInstances = attributes.value.fieldValues[attributes.filterKey];
        return this.getAppInstancesAdvancedFilter(appInstances.map(i => i.id), attributes.operator, encapsulate);
      case EnvironmentAdvancedFilterKey.AppInstance:
        const appInstance = attributes.value.fieldValues[attributes.filterKey];
        return this.getAppInstancesAdvancedFilter([appInstance.id], attributes.operator, encapsulate);
      case EnvironmentAdvancedFilterKey.Countries:
        const countries = attributes.value.fieldValues[attributes.filterKey];
        return this.getCountriesAdvancedFilter(countries.map(c => c.id), attributes.operator, encapsulate);
      case EnvironmentAdvancedFilterKey.Country:
        const country = attributes.value.fieldValues[attributes.filterKey];
        return this.getCountriesAdvancedFilter([country.id], attributes.operator, encapsulate);
      case EnvironmentAdvancedFilterKey.CreatedAt:
        return this.getCreatedAtAdvancedFilter(attributes, encapsulate);
      case EnvironmentAdvancedFilterKey.Distributor:
        const distributor = attributes.value.fieldValues[attributes.filterKey];
        return this.getDistributorsAdvancedFilter([distributor.id], attributes.operator, encapsulate);
      case EnvironmentAdvancedFilterKey.Distributors:
        const distributors = attributes.value.fieldValues[attributes.filterKey];
        return this.getDistributorsAdvancedFilter(distributors.map(d => d.id), attributes.operator, encapsulate);
      case EnvironmentAdvancedFilterKey.Cluster:
        return this.getClusterAdvancedFilter(attributes, encapsulate);
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
      applicationIds: number[],
      op: ComparisonOperator,
      encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const { operator, itemsMatched, logicalOperator } = AdvancedFilterOperatorMapping.getListComparisonOperatorDto(op);
    const toFilterCriterion = (c) => encapsulate({
      appInstances: { applicationId: c, itemsMatched },
    });

    return AdvancedFilterTypeMapping.toAdvancedFilter(applicationIds, operator, toFilterCriterion, logicalOperator);
  }

  private getCountriesAdvancedFilter(
    countryIds: number[],
    op: ComparisonOperator,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const { operator, itemsMatched, logicalOperator } = AdvancedFilterOperatorMapping.getListComparisonOperatorDto(op);
    const toFilterCriterion = c => (encapsulate({
      legalUnits: { countryId: c, itemsMatched },
    }));

    return AdvancedFilterTypeMapping.toAdvancedFilter(countryIds, operator, toFilterCriterion, logicalOperator);
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

  private getDistributorsAdvancedFilter(
    distributorIds: number[],
    op: ComparisonOperator,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const { operator, itemsMatched, logicalOperator } = AdvancedFilterOperatorMapping.getListComparisonOperatorDto(op);
    const toFilterCriterion = c => (encapsulate({
      distributors: { id: c, itemsMatched },
    }));

    return AdvancedFilterTypeMapping.toAdvancedFilter(distributorIds, operator, toFilterCriterion, logicalOperator);
  }

  private getClusterAdvancedFilter(attributes: IAdvancedFilterAttributes, encapsulate: IFilterCriterionEncapsulation): AdvancedFilter {
    const clusterIds = attributes.value.fieldValues[attributes.filterKey].map((d: IDistributor) => d.id);

    const { operator, itemsMatched, logicalOperator } = AdvancedFilterOperatorMapping.getListComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => (encapsulate({
      distributorId: c,
      itemsMatched,
    }));

    return AdvancedFilterTypeMapping.toAdvancedFilter(clusterIds, operator, toFilterCriterion, logicalOperator);
  }
}
