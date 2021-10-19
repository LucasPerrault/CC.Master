import { Injectable } from '@angular/core';
import { IDistributor } from '@cc/domain/billing/distributors';
import { IEnvironment, IEnvironmentDomain } from '@cc/domain/environments';

import {
  AdvancedFilter,
  AdvancedFilterFormMapping,
  AdvancedFilterTypeMapping,
  ComparisonOperator,
  getComparisonBooleanValue,
  IAdvancedFilterAttributes,
  IAdvancedFilterForm,
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

  public getAdvancedFilter(attributes: IAdvancedFilterAttributes): AdvancedFilter {
    switch (attributes.filterKey) {
      case EnvironmentAdvancedFilterKey.Domain:
        const domains = attributes.value[attributes.filterKey];
        return this.getDomainAdvancedFilter(attributes.operator, domains);
      case EnvironmentAdvancedFilterKey.IsActive:
        return this.getIsActiveAdvancedFilter(attributes.operator);
      case EnvironmentAdvancedFilterKey.Subdomain:
        const subdomains = attributes.value[attributes.filterKey];
        return this.getSubdomainAdvancedFilter(attributes.operator, subdomains);
      case EnvironmentAdvancedFilterKey.AppInstances:
        const applications = attributes.value[attributes.filterKey];
        return this.getAppInstanceAdvancedFilter(attributes.operator, applications);
      case EnvironmentAdvancedFilterKey.Countries:
        const countries = attributes.value[attributes.filterKey];
        return this.getCountriesAdvancedFilter(attributes.operator, countries);
      case EnvironmentAdvancedFilterKey.CreatedAt:
        const createdAt = attributes.value[attributes.filterKey];
        return this.getCreatedAtAdvancedFilter(attributes.operator, createdAt);
      case EnvironmentAdvancedFilterKey.Distributors:
        const distributors = attributes.value[attributes.filterKey];
        return this.getDistributorsAdvancedFilter(attributes.operator, distributors);
    }
  }

  private getDomainAdvancedFilter(operator: ComparisonOperator, domains: IEnvironmentDomain[]): AdvancedFilter {
    const criterions = AdvancedFilterTypeMapping.toCriterions(
        operator,
        domains.map(a => a.id),
        c => ({ domain: c }),
    );
    return AdvancedFilterTypeMapping.combine(criterions, LogicalOperator.And);
  }

  private getIsActiveAdvancedFilter(operator: ComparisonOperator): AdvancedFilter {
    const query = getComparisonBooleanValue(operator);
    const comparison = AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, query);

    return AdvancedFilterTypeMapping.toFilterCriterion({
      isActive: comparison,
    });
  }

  private getSubdomainAdvancedFilter(operator: ComparisonOperator, environments: IEnvironment[]): AdvancedFilter {
    const criterions = AdvancedFilterTypeMapping.toCriterions(
        operator,
        environments.map(a => a.subDomain),
        c => ({ subdomain: c }),
    );
    return AdvancedFilterTypeMapping.combine(criterions, LogicalOperator.And);
  }

  private getAppInstanceAdvancedFilter(operator: ComparisonOperator, appInstances: IApplication[]): AdvancedFilter {
    const criterions = AdvancedFilterTypeMapping.toCriterions(
        operator,
        appInstances.map(a => a.id),
        c => ({ appInstance: { applicationId: c } }),
    );
    return AdvancedFilterTypeMapping.combine(criterions, LogicalOperator.Or);
  }

  private getCountriesAdvancedFilter(operator: ComparisonOperator, countries: ICountry[]): AdvancedFilter {

    const criterions = AdvancedFilterTypeMapping.toCriterions(
        operator,
        countries.map(c => c.id),
        c => ({ legalUnits: { countryId: c } }),
    );
    return AdvancedFilterTypeMapping.combine(criterions, LogicalOperator.And);

  }

  private getCreatedAtAdvancedFilter(operator: ComparisonOperator, createdAt: string): AdvancedFilter {
    const comparison = AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, createdAt);

    return AdvancedFilterTypeMapping.toFilterCriterion({
      createdAt: comparison,
    });
  }

  private getDistributorsAdvancedFilter(operator: ComparisonOperator, distributors: IDistributor[]): AdvancedFilter {
    const criterions = AdvancedFilterTypeMapping.toCriterions(
      operator,
      distributors.map(a => a.id),
      c => ({ distributorId: c }),
    );
    return AdvancedFilterTypeMapping.combine(criterions, LogicalOperator.Or);
  }
}
