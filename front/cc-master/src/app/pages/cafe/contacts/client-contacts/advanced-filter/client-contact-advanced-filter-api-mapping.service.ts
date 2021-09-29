import { Injectable } from '@angular/core';

import {
  AdvancedFilter,
  AdvancedFilterTypeMapping,
  ComparisonOperator,
  IAdvancedFilterForm,
  IComparisonFilterCriterionForm,
  IComparisonValue,
  LogicalOperator,
} from '../../../common/cafe-filters/advanced-filter-form';
import { ClientContactAdvancedFilterKey } from './client-contact-advanced-filter-key.enum';
import { IClient } from '@cc/domain/billing/clients';

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
        const subdomain = attributes.value[attributes.filterKey];
        return this.getSubdomainAdvancedFilter(attributes.operator, subdomain);
    }
  }

  private getClientsAdvancedFilter(operator: ComparisonOperator, clients: IClient[]): AdvancedFilter {
    const queries = clients.map(c => `${ c.id }`);
    const comparisons = queries.map(q => AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, q));

    const criterions = comparisons.map(c => AdvancedFilterTypeMapping.toFilterCriterion({
      client: c,
    }));

    return AdvancedFilterTypeMapping.toFilterCombination(LogicalOperator.And, criterions);
  }

  private getIsConfirmedAdvancedFilter(operator: ComparisonOperator): AdvancedFilter {
    const comparison = AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, operator);

    return AdvancedFilterTypeMapping.toFilterCriterion({
      isConfirmed: comparison,
    });
  }

  private getSubdomainAdvancedFilter(operator: ComparisonOperator, subdomain?: string): AdvancedFilter {
    const comparison = AdvancedFilterTypeMapping.toComparisonFilterCriterion(operator, subdomain);

    return AdvancedFilterTypeMapping.toFilterCriterion({
      subdomain: comparison,
    });
  }
}
