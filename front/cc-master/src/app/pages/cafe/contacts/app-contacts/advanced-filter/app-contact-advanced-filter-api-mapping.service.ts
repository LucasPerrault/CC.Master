import { Injectable } from '@angular/core';

import {
  AdvancedFilter,
  AdvancedFilterTypeMapping,
  ComparisonOperator,
  getComparisonBooleanValue,
  IAdvancedFilterForm,
  IComparisonFilterCriterionForm,
  IComparisonValue,
  LogicalOperator,
} from '../../../common/cafe-filters/advanced-filter-form';
import { IAppInstance } from '../../../environments/models/app-instance.interface';
import { AppContactAdvancedFilterKey } from './app-contact-advanced-filter-key.enum';
import { IEnvironment } from '@cc/domain/environments';

interface IAdvancedFilterAttributes {
  filterKey: string;
  operator: ComparisonOperator;
  value?: IComparisonValue;
}

const toAdvancedFilterAttributes = (form: IComparisonFilterCriterionForm): IAdvancedFilterAttributes =>
  ({ filterKey: form?.criterion?.key, operator: form?.operator?.id, value: form?.values });

@Injectable()
export class AppContactAdvancedFilterApiMappingService {
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
      case AppContactAdvancedFilterKey.EnvironmentApplications:
        const envAppInstances = attributes.value[attributes.filterKey];
        return this.getEnvironmentAppInstancesAdvancedFilter(attributes.operator, envAppInstances);
      case AppContactAdvancedFilterKey.Applications:
        const appInstances = attributes.value[attributes.filterKey];
        return this.getAppInstanceAdvancedFilter(attributes.operator, appInstances);
      case AppContactAdvancedFilterKey.IsConfirmed:
        return this.getIsConfirmedAdvancedFilter(attributes.operator);
      case AppContactAdvancedFilterKey.Subdomain:
        const subdomains = attributes.value[attributes.filterKey];
        return this.getSubdomainAdvancedFilter(attributes.operator, subdomains);
    }
  }

  private getEnvironmentAppInstancesAdvancedFilter(operator: ComparisonOperator, appInstances: IAppInstance[]): AdvancedFilter {
    const queries = appInstances.map(a => `${ a.id }`);
    const comparisons = queries.map(q => AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, q));

    const criterions = comparisons.map(c => AdvancedFilterTypeMapping.toFilterCriterion({
      environment: {
        appInstances: {
          applicationId: c,
        },
      },
    }));

    return AdvancedFilterTypeMapping.toFilterCombination(LogicalOperator.And, criterions);
  }

  private getAppInstanceAdvancedFilter(operator: ComparisonOperator, appInstances: IAppInstance[]): AdvancedFilter {
    const queries = appInstances.map(a => `${ a.id }`);
    const comparisons = queries.map(q => AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, q));

    const criterions = comparisons.map(c => AdvancedFilterTypeMapping.toFilterCriterion({
      appInstances: {
        applicationId: c,
      },
    }));

    return AdvancedFilterTypeMapping.toFilterCombination(LogicalOperator.And, criterions);
  }

  private getIsConfirmedAdvancedFilter(operator: ComparisonOperator): AdvancedFilter {
    const query = getComparisonBooleanValue(operator);
    const comparison = AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, query);

    return AdvancedFilterTypeMapping.toFilterCriterion({
      isConfirmed: comparison,
    });
  }

  private getSubdomainAdvancedFilter(operator: ComparisonOperator, subdomains: IEnvironment[]): AdvancedFilter {
    const queries = subdomains.map(a => `${ a.subDomain }`);
    const comparisons = queries.map(q => AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, q));

    const criterions = comparisons.map(c => AdvancedFilterTypeMapping.toFilterCriterion({
      subdomain: c,
    }));

    return AdvancedFilterTypeMapping.toFilterCombination(LogicalOperator.And, criterions);
  }
}
