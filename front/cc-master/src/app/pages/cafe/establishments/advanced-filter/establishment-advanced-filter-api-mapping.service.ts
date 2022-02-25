import { Injectable } from '@angular/core';

import {
  AdvancedFilter,
  AdvancedFilterFormMapping,
  AdvancedFilterTypeMapping,
  IAdvancedFilterAttributes,
  IAdvancedFilterForm,
} from '../../common/components/advanced-filter-form';
import { AdvancedFilterOperatorMapping } from '../../common/components/advanced-filter-form';
import { ICountry } from '../../common/models/legal-unit.interface';
import { EnvironmentAdvancedFilterApiMappingService } from '../../environments/advanced-filter';
import { EstablishmentAdvancedFilterKey } from './establishment-advanced-filter-key.enum';

@Injectable()
export class EstablishmentAdvancedFilterApiMappingService {

  constructor(private environmentApiMapping: EnvironmentAdvancedFilterApiMappingService) {}

  public toAdvancedFilter(advancedFilterForm: IAdvancedFilterForm): AdvancedFilter {
    return AdvancedFilterFormMapping.toAdvancedFilter(advancedFilterForm, a => this.getAdvancedFilter(a));
  }

  public getAdvancedFilter(attributes: IAdvancedFilterAttributes): AdvancedFilter {
    switch (attributes.filterKey) {
      case EstablishmentAdvancedFilterKey.Country:
        return this.getCountryAdvancedFilter(attributes);
      default:
        return this.getEnvironmentAdvancedFilter(attributes);
    }
  }

  private getCountryAdvancedFilter(attributes: IAdvancedFilterAttributes): AdvancedFilter {
    const countries = attributes.value.fieldValues[attributes.filterKey].map((c: ICountry) => c.id);
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => ({ legalUnit: { countryId: c } });

    return AdvancedFilterTypeMapping.toAdvancedFilter(countries, operator, toFilterCriterion, logicalOperator);
  }

  private getEnvironmentAdvancedFilter(attributes: IAdvancedFilterAttributes): AdvancedFilter {
    const toFilterCriterion = criterion => ({ environment: criterion });
    return this.environmentApiMapping.getAdvancedFilter(attributes, toFilterCriterion);
  }
}
