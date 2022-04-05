import { Injectable } from '@angular/core';

import {
  AdvancedFilter,
  AdvancedFilterFormMapping,
  AdvancedFilterOperatorMapping,
  AdvancedFilterTypeMapping,
  cast,
  defaultEncapsulation,
  IAdvancedCriterionAttributes,
  IAdvancedFilterForm,
  IComparisonCriterionAttributes,
  IFilterCriterionEncapsulation,
  ItemsMatchedDto,
} from '../../common/components/advanced-filter-form';
import { IEnvironment } from '../../common/models/environment.interface';
import { FacetAdvancedFilterApiMappingService } from '../../common/services/facets/facet-advanced-filter-api-mapping.service';
import { EnvironmentAdvancedFilterKey } from './environment-advanced-filter-key.enum';
import { EnvironmentCriterionKey } from './environment-criterion-key.enum';

@Injectable()
export class EnvironmentAdvancedFilterApiMappingService {

  constructor(private facetFiltersApiMapping: FacetAdvancedFilterApiMappingService) {}

  public toAdvancedFilter(advancedFilterForm: IAdvancedFilterForm): AdvancedFilter {
    return AdvancedFilterFormMapping.toAdvancedFilter(advancedFilterForm, a => this.getAdvancedFilter(a));
  }

  public getAdvancedFilter(
    attributes: IAdvancedCriterionAttributes,
    encapsulate: IFilterCriterionEncapsulation = defaultEncapsulation,
  ): AdvancedFilter {
    switch (attributes.criterionKey) {
      case EnvironmentCriterionKey.Subdomain:
        return this.getSubdomainAdvancedFilter(cast<IComparisonCriterionAttributes>(attributes.content), encapsulate);
      case EnvironmentCriterionKey.AppInstances:
        return this.getAppInstancesAdvancedFilter(cast<IComparisonCriterionAttributes>(attributes.content), encapsulate);
      case EnvironmentCriterionKey.Countries:
        return this.getCountriesAdvancedFilter(cast<IComparisonCriterionAttributes>(attributes.content), encapsulate);
      case EnvironmentCriterionKey.CreatedAt:
        return this.getCreatedAtAdvancedFilter(cast<IComparisonCriterionAttributes>(attributes.content), encapsulate);
      case EnvironmentCriterionKey.Distributors:
        return this.getDistributorsAdvancedFilter(cast<IComparisonCriterionAttributes>(attributes.content), encapsulate);
      case EnvironmentCriterionKey.BillingEntity:
        return this.getBillingEntitiesAdvancedFilter(cast<IComparisonCriterionAttributes>(attributes.content), encapsulate);
      case EnvironmentCriterionKey.Cluster:
        return this.getClusterAdvancedFilter(cast<IComparisonCriterionAttributes>(attributes.content), encapsulate);
      case EnvironmentCriterionKey.DistributorType:
        return this.getDistributorTypeAdvancedFilter(cast<IComparisonCriterionAttributes>(attributes.content), encapsulate);
      case EnvironmentCriterionKey.Facets:
        return this.getFacetAdvancedFilter(cast<IAdvancedCriterionAttributes>(attributes.content), encapsulate);
    }
  }

  private getSubdomainAdvancedFilter(
    attributes: IComparisonCriterionAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const subdomains = attributes.value[attributes.filterKey].map((e: IEnvironment) => e.subdomain);
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => (encapsulate({ subdomain: c }));

    return AdvancedFilterTypeMapping.toAdvancedFilter(subdomains, operator, toFilterCriterion, logicalOperator);
  }

  private getAppInstancesAdvancedFilter(attributes: IComparisonCriterionAttributes, encapsulate: IFilterCriterionEncapsulation) {
    const appInstanceIds = this.getAppInstanceIds(attributes);
    const { operator, itemsMatched, logicalOperator } = AdvancedFilterOperatorMapping.getListComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = (c) => encapsulate({
      appInstances: { applicationId: c, itemsMatched },
    });

    return AdvancedFilterTypeMapping.toAdvancedFilter(appInstanceIds, operator, toFilterCriterion, logicalOperator);
  }

  private getAppInstanceIds(attributes: IComparisonCriterionAttributes): number[] {
    switch (attributes.filterKey) {
      case EnvironmentAdvancedFilterKey.AppInstances:
        const appInstances = attributes.value[EnvironmentAdvancedFilterKey.AppInstances];
        return appInstances?.map(a => a?.id);
      case EnvironmentAdvancedFilterKey.AppInstance:
        const appInstance = attributes.value[EnvironmentAdvancedFilterKey.AppInstance];
        return [appInstance?.id];
    }
  }

  private getCountriesAdvancedFilter(
    attributes: IComparisonCriterionAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const countryIds = this.getCountryIds(attributes);
    const { operator, itemsMatched, logicalOperator } = AdvancedFilterOperatorMapping.getListComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => (encapsulate({
      legalUnits: { countryId: c, itemsMatched },
    }));

    return AdvancedFilterTypeMapping.toAdvancedFilter(countryIds, operator, toFilterCriterion, logicalOperator);
  }

  private getCountryIds(attributes: IComparisonCriterionAttributes): number[] {
    switch (attributes.filterKey) {
      case EnvironmentAdvancedFilterKey.Countries:
        const countries = attributes.value[EnvironmentAdvancedFilterKey.Countries];
        return countries?.map(c => c.id);
      case EnvironmentAdvancedFilterKey.Country:
        const country = attributes.value[EnvironmentAdvancedFilterKey.Country];
        return [country.id];
    }
  }

  private getCreatedAtAdvancedFilter(
    attributes: IComparisonCriterionAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const createdAt = attributes.value[attributes.filterKey];
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => (encapsulate({ createdAt: c }));

    return AdvancedFilterTypeMapping.toAdvancedFilter([createdAt], operator, toFilterCriterion, logicalOperator);
  }

  private getDistributorsAdvancedFilter(
    attributes: IComparisonCriterionAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const distributorIds = this.getDistributorIds(attributes);
    const { operator, itemsMatched, logicalOperator } = AdvancedFilterOperatorMapping.getListComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => (encapsulate({
      distributors: { id: c, itemsMatched },
    }));

    return AdvancedFilterTypeMapping.toAdvancedFilter(distributorIds, operator, toFilterCriterion, logicalOperator);
  }

  private getDistributorIds(attributes: IComparisonCriterionAttributes): number[] {
    switch (attributes.filterKey) {
      case EnvironmentAdvancedFilterKey.Distributor:
        const distributor = attributes.value[EnvironmentAdvancedFilterKey.Distributor];
        return [distributor?.id];
      case EnvironmentAdvancedFilterKey.Distributors:
        const distributors = attributes.value[EnvironmentAdvancedFilterKey.Distributors];
        return distributors.map(d => d.id);
    }
  }

  private getClusterAdvancedFilter(
    attributes: IComparisonCriterionAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const clusterIds = attributes.value[attributes.filterKey];
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => (encapsulate({
      cluster: c,
    }));

    return AdvancedFilterTypeMapping.toAdvancedFilter(clusterIds, operator, toFilterCriterion, logicalOperator);
  }

  private getDistributorTypeAdvancedFilter(
    attributes: IComparisonCriterionAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const distributorType = attributes.value[attributes.filterKey];
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => (encapsulate({ distributorType: c }));

    return AdvancedFilterTypeMapping.toAdvancedFilter([distributorType], operator, toFilterCriterion, logicalOperator);
  }

  private getBillingEntitiesAdvancedFilter(
    attributes: IComparisonCriterionAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const billingEntities = attributes.value[attributes.filterKey]?.map(b => b.id);
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

  private getFacetAdvancedFilter(attributes: IAdvancedCriterionAttributes, encapsulate: IFilterCriterionEncapsulation): AdvancedFilter {
    return this.facetFiltersApiMapping.getAdvancedFilter(attributes, encapsulate);
  }
}
