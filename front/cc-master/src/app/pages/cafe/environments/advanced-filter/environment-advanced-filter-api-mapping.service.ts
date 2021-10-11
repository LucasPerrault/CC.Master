import { Injectable } from '@angular/core';
import { IEnvironment, IEnvironmentDomain } from '@cc/domain/environments';

import {
  AdvancedFilter, AdvancedFilterTypeMapping,
  ComparisonOperator, getComparisonBooleanValue,
  IAdvancedFilterForm,
  IComparisonFilterCriterionForm,
  IComparisonValue,
  LogicalOperator,
} from '../../common/cafe-filters/advanced-filter-form';
import { IAppInstance } from '../models/app-instance.interface';
import { ICountry } from '../models/legal-unit.interface';
import { EnvironmentAdvancedFilterKey } from './environment-advanced-filter-key.enum';

interface IAdvancedFilterAttributes {
  filterKey: string;
  operator: ComparisonOperator;
  value?: IComparisonValue;
}

const toAdvancedFilterAttributes = (form: IComparisonFilterCriterionForm): IAdvancedFilterAttributes =>
  ({ filterKey: form?.criterion?.key, operator: form?.operator?.id, value: form?.values });

@Injectable()
export class EnvironmentAdvancedFilterApiMappingService {
  public toAdvancedFilter(advancedFilterForm: IAdvancedFilterForm): AdvancedFilter {
    if (!advancedFilterForm?.logicalOperator && !advancedFilterForm?.criterionForms?.length) {
      return;
    }

    const filtersCriterion: AdvancedFilter[] = advancedFilterForm.criterionForms
      .map(form => toAdvancedFilterAttributes(form))
      .filter(attributes => !!attributes?.filterKey && !!attributes?.operator)
      .map(attributes => this.getAdvancedFilter(attributes));

    if (!advancedFilterForm.logicalOperator || filtersCriterion.length === 1) {
      return filtersCriterion[0];
    }

    return AdvancedFilterTypeMapping.toFilterCombination(advancedFilterForm.logicalOperator.id, filtersCriterion);
  }

  private getAdvancedFilter(attributes: IAdvancedFilterAttributes): AdvancedFilter {
    switch (attributes.filterKey) {
      case EnvironmentAdvancedFilterKey.Domain:
        const domains = attributes.value[attributes.filterKey];
        return this.getDomainAdvancedFilter(attributes.operator, domains);
      case EnvironmentAdvancedFilterKey.IsActive:
        return this.getIsActiveAdvancedFilter(attributes.operator);
      case EnvironmentAdvancedFilterKey.Subdomain:
        const subdomains = attributes.value[attributes.filterKey];
        return this.getSubdomainAdvancedFilter(attributes.operator, subdomains);
      case EnvironmentAdvancedFilterKey.Applications:
        const applications = attributes.value[attributes.filterKey];
        return this.getAppInstanceAdvancedFilter(attributes.operator, applications);
      case EnvironmentAdvancedFilterKey.Countries:
        const countries = attributes.value[attributes.filterKey];
        return this.getCountriesAdvancedFilter(attributes.operator, countries);
    }
  }

  private getDomainAdvancedFilter(operator: ComparisonOperator, domains: IEnvironmentDomain[]): AdvancedFilter {
    const queries = domains.map(a => `${ a.id }`);
    const comparisons = queries.map(q => AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, q));

    const criterions = comparisons.map(c => AdvancedFilterTypeMapping.toFilterCriterion({
      domain: c,
    }));

    return !!criterions.length && criterions.length > 1
      ? AdvancedFilterTypeMapping.toFilterCombination(LogicalOperator.And, criterions)
      : criterions[0];
  }

  private getIsActiveAdvancedFilter(operator: ComparisonOperator): AdvancedFilter {
    const query = getComparisonBooleanValue(operator);
    const comparison = AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, query);

    return AdvancedFilterTypeMapping.toFilterCriterion({
      isActive: comparison,
    });
  }

  private getSubdomainAdvancedFilter(operator: ComparisonOperator, subdomains: IEnvironment[]): AdvancedFilter {
    const queries = subdomains.map(a => `${ a.subDomain }`);
    const comparisons = queries.map(q => AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, q));

    const criterions = comparisons.map(c => AdvancedFilterTypeMapping.toFilterCriterion({
      subdomain: c,
    }));

    return !!criterions.length && criterions.length > 1
      ? AdvancedFilterTypeMapping.toFilterCombination(LogicalOperator.And, criterions)
      : criterions[0];
  }

  private getAppInstanceAdvancedFilter(operator: ComparisonOperator, appInstances: IAppInstance[]): AdvancedFilter {
    const queries = appInstances.map(a => `${ a.id }`);
    const comparisons = queries.map(q => AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, q));

    const criterions = comparisons.map(c => AdvancedFilterTypeMapping.toFilterCriterion({
      appInstances: {
        applicationId: c,
      },
    }));

    return !!criterions.length && criterions.length > 1
      ? AdvancedFilterTypeMapping.toFilterCombination(LogicalOperator.And, criterions)
      : criterions[0];
  }

  private getCountriesAdvancedFilter(operator: ComparisonOperator, countries: ICountry[]): AdvancedFilter {
    const queries = countries.map(a => a.id);
    const comparisons = queries.map(q => AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, q));

    const criterions = comparisons.map(c => AdvancedFilterTypeMapping.toFilterCriterion({
      legalUnits: { countryId: c },
    }));

    return !!criterions.length && criterions.length > 1
      ? AdvancedFilterTypeMapping.toFilterCombination(LogicalOperator.And, criterions)
      : criterions[0];
  }
}
