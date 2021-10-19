import { Injectable } from '@angular/core';
import { IDistributor } from '@cc/domain/billing/distributors';
import { IEnvironment, IEnvironmentDomain } from '@cc/domain/environments';

import {
  AdvancedFilter,
  AdvancedFilterFormMapping,
  AdvancedFilterTypeMapping,
  ComparisonOperator,
  defaultEncapsulation,
  getComparisonBooleanValue,
  IAdvancedFilterAttributes,
  IAdvancedFilterForm,
  IComparisonFilterCriterionEncapsulation,
  IFilterCriterionEncapsulation,
  LogicalOperator,
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
      case EnvironmentAdvancedFilterKey.Domain:
        return this.getDomainAdvancedFilter(attributes, c => (encapsulate({ domain: c })));
      case EnvironmentAdvancedFilterKey.IsActive:
        return this.getIsActiveAdvancedFilter(attributes.operator, c => (encapsulate({ isActive: c })));
      case EnvironmentAdvancedFilterKey.Subdomain:
        return this.getSubdomainAdvancedFilter(attributes, c => (encapsulate({ subdomain: c })));
      case EnvironmentAdvancedFilterKey.AppInstances:
        return this.getAppInstanceAdvancedFilter(attributes, c => (encapsulate({ appInstances: { applicationId: c } })));
      case EnvironmentAdvancedFilterKey.Countries:
        return this.getCountriesAdvancedFilter(attributes, c => (encapsulate({ legalUnits: { countryId: c } })));
      case EnvironmentAdvancedFilterKey.CreatedAt:
        return this.getCreatedAtAdvancedFilter(attributes, c => (encapsulate({ createdAt: c })));
      case EnvironmentAdvancedFilterKey.Distributors:
        return this.getDistributorsAdvancedFilter(attributes, c => (encapsulate({ distributorId: c })));
    }
  }

  private getDomainAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    toIFilterCriterion: IComparisonFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const domainIds = attributes.value[attributes.filterKey]?.map((d: IEnvironmentDomain) => d.id);
    const criterions = AdvancedFilterTypeMapping.toCriterions(attributes.operator, domainIds, toIFilterCriterion);
    return AdvancedFilterTypeMapping.combine(criterions, LogicalOperator.And);
  }

  private getIsActiveAdvancedFilter(
    operator: ComparisonOperator,
    toIFilterCriterion: IComparisonFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const query = getComparisonBooleanValue(operator);
    const comparison = AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, query);
    return AdvancedFilterTypeMapping.toFilterCriterion(toIFilterCriterion(comparison));
  }

  private getSubdomainAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    toIFilterCriterion: IComparisonFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const subdomains = attributes.value[attributes.filterKey].map((e: IEnvironment) => e.subDomain);
    const criterions = AdvancedFilterTypeMapping.toCriterions(attributes.operator, subdomains, toIFilterCriterion);
    return AdvancedFilterTypeMapping.combine(criterions, LogicalOperator.Or);
  }

  private getAppInstanceAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    toIFilterCriterion: IComparisonFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const applicationIds = attributes.value[attributes.filterKey].map((a: IApplication) => a.id);
    const criterions = AdvancedFilterTypeMapping.toCriterions(attributes.operator, applicationIds, toIFilterCriterion);
    return AdvancedFilterTypeMapping.combine(criterions, LogicalOperator.Or);
  }

  private getCountriesAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    toIFilterCriterion: IComparisonFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const countryIds = attributes.value[attributes.filterKey].map((c: ICountry) => c.id);
    const criterions = AdvancedFilterTypeMapping.toCriterions(attributes.operator, countryIds, toIFilterCriterion);
    return AdvancedFilterTypeMapping.combine(criterions, LogicalOperator.Or);
  }

  private getCreatedAtAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    toIFilterCriterion: IComparisonFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const createdAt = attributes.value[attributes.filterKey];
    const comparison = AdvancedFilterTypeMapping.toComparisonFilterCriterion(attributes.operator, createdAt);
    return AdvancedFilterTypeMapping.toFilterCriterion(toIFilterCriterion(comparison));
  }

  private getDistributorsAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    toIFilterCriterion: IComparisonFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const distributorIds = attributes.value[attributes.filterKey]?.map((d: IDistributor) => d.id);
    const criterions = AdvancedFilterTypeMapping.toCriterions(attributes.operator, distributorIds, toIFilterCriterion);
    return AdvancedFilterTypeMapping.combine(criterions, LogicalOperator.Or);
  }
}
