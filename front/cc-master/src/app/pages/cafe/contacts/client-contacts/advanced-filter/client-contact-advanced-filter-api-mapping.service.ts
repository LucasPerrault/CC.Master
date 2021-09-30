import { Injectable } from '@angular/core';
import { IClient } from '@cc/domain/billing/clients';
import { IEnvironment } from '@cc/domain/environments';

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
import { ClientContactAdvancedFilterKey } from './client-contact-advanced-filter-key.enum';

interface IAdvancedFilterAttributes {
  filterKey: string;
  operator: ComparisonOperator;
  value?: IComparisonValue;
}

const toAdvancedFilterAttributes = (form: IComparisonFilterCriterionForm): IAdvancedFilterAttributes =>
  ({ filterKey: form?.criterion?.key, operator: form?.operator?.id, value: form?.values });

@Injectable()
export class ClientContactAdvancedFilterApiMappingService {
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
      case ClientContactAdvancedFilterKey.Clients:
        const clients = attributes.value[attributes.filterKey];
        return this.getClientsAdvancedFilter(attributes.operator, clients);
      case ClientContactAdvancedFilterKey.IsConfirmed:
        return this.getIsConfirmedAdvancedFilter(attributes.operator);
      case ClientContactAdvancedFilterKey.Subdomain:
        const subdomains = attributes.value[attributes.filterKey];
        return this.getSubdomainAdvancedFilter(attributes.operator, subdomains);
    }
  }

  private getClientsAdvancedFilter(operator: ComparisonOperator, clients: IClient[]): AdvancedFilter {
    const queries = clients.map(c => `${ c.id }`);
    const comparisons = queries.map(q => AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, q));

    const criterions = comparisons.map(c => AdvancedFilterTypeMapping.toFilterCriterion({
      client: c,
    }));

    return !!criterions.length && criterions.length > 1
      ? AdvancedFilterTypeMapping.toFilterCombination(LogicalOperator.And, criterions)
      : criterions[0];
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

    return !!criterions.length && criterions.length > 1
      ? AdvancedFilterTypeMapping.toFilterCombination(LogicalOperator.And, criterions)
      : criterions[0];
  }
}
