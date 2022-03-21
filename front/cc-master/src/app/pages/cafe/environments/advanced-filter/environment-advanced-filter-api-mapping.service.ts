import { Injectable } from '@angular/core';

import {
  AdvancedFilter,
  AdvancedFilterFormMapping,
  AdvancedFilterOperatorMapping,
  AdvancedFilterTypeMapping,
  ComparisonOperator,
  defaultEncapsulation,
  IAdvancedFilterAttributes,
  IAdvancedFilterForm,
  IFilterCriterionEncapsulation,
  ItemsMatchedDto,
} from '../../common/components/advanced-filter-form';
import { IEnvironment } from '../../common/models/environment.interface';
import { FacetAdvancedFilterApiMappingService } from '../../common/services/facets/facet-advanced-filter-api-mapping.service';
import { EnvironmentAdvancedFilterKey } from './environment-advanced-filter-key.enum';

@Injectable()
export class EnvironmentAdvancedFilterApiMappingService {

  constructor(private facetFiltersApiMapping: FacetAdvancedFilterApiMappingService) {}

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
      case EnvironmentAdvancedFilterKey.BillingEntities:
        return this.getBillingEntitiesAdvancedFilter(attributes, encapsulate);
      case EnvironmentAdvancedFilterKey.Cluster:
        return this.getClusterAdvancedFilter(attributes, encapsulate);
      case EnvironmentAdvancedFilterKey.DistributorType:
        return this.getDistributorTypeAdvancedFilter(attributes, encapsulate);
      default:
        return this.getFacetAdvancedFilter(attributes, encapsulate);
    }
  }

  private getSubdomainAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const subdomains = attributes.value.fieldValues[attributes.filterKey].map((e: IEnvironment) => e.subdomain);
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => (encapsulate({ subdomain: c }));

    return AdvancedFilterTypeMapping.toAdvancedFilter(subdomains, operator, toFilterCriterion, logicalOperator);
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
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => (encapsulate({ createdAt: c }));

    return AdvancedFilterTypeMapping.toAdvancedFilter([createdAt], operator, toFilterCriterion, logicalOperator);
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
    const clusterIds = attributes.value.fieldValues[attributes.filterKey];
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => (encapsulate({
      cluster: c,
    }));

    return AdvancedFilterTypeMapping.toAdvancedFilter(clusterIds, operator, toFilterCriterion, logicalOperator);
  }

  private getDistributorTypeAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const distributorType = attributes.value.fieldValues[attributes.filterKey];
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => (encapsulate({ distributorType: c }));

    return AdvancedFilterTypeMapping.toAdvancedFilter([distributorType], operator, toFilterCriterion, logicalOperator);
  }

  private getBillingEntitiesAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const billingEntities = attributes.value.fieldValues[attributes.filterKey]?.map(b => b.id);
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => (encapsulate({
      contracts: {
        client: {
          billingEntity: c,
        },
        itemsMatched: ItemsMatchedDto.All,
      },
    }));

    return AdvancedFilterTypeMapping.toAdvancedFilter(billingEntities, operator, toFilterCriterion, logicalOperator);
  }

  private getFacetAdvancedFilter(attributes: IAdvancedFilterAttributes, encapsulate: IFilterCriterionEncapsulation): AdvancedFilter {
    return this.facetFiltersApiMapping.getAdvancedFilter(attributes, encapsulate);
  }
}
