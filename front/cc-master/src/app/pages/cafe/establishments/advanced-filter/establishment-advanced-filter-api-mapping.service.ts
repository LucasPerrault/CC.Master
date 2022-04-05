import { Injectable } from '@angular/core';

import {
  AdvancedFilter,
  AdvancedFilterFormMapping,
  AdvancedFilterTypeMapping, cast, IAdvancedCriterionAttributes,
  IAdvancedFilterForm, IComparisonCriterionAttributes,
} from '../../common/components/advanced-filter-form';
import { AdvancedFilterOperatorMapping } from '../../common/components/advanced-filter-form';
import { ICountry } from '../../common/models/legal-unit.interface';
import { FacetAdvancedFilterApiMappingService } from '../../common/services/facets/facet-advanced-filter-api-mapping.service';
import { EnvironmentAdvancedFilterApiMappingService } from '../../environments/advanced-filter';
import { EstablishmentCriterionKey } from './establishment-criterion-key.enum';

@Injectable()
export class EstablishmentAdvancedFilterApiMappingService {

  constructor(
    private environmentApiMapping: EnvironmentAdvancedFilterApiMappingService,
    private facetApiMapping: FacetAdvancedFilterApiMappingService,
  ) {}

  public toAdvancedFilter(advancedFilterForm: IAdvancedFilterForm): AdvancedFilter {
    return AdvancedFilterFormMapping.toAdvancedFilter(advancedFilterForm, a => this.getAdvancedFilter(a));
  }

  public getAdvancedFilter(attributes: IAdvancedCriterionAttributes): AdvancedFilter {
    switch (attributes.criterionKey) {
      case EstablishmentCriterionKey.Country:
        return this.getCountryAdvancedFilter(cast<IComparisonCriterionAttributes>(attributes.content));
      case EstablishmentCriterionKey.Environment:
        const toFilterCriterion = criterion => ({ environment: criterion });
        return this.environmentApiMapping.getAdvancedFilter(cast<IAdvancedCriterionAttributes>(attributes.content), toFilterCriterion);
      case EstablishmentCriterionKey.Facet:
        return this.facetApiMapping.getAdvancedFilter(cast<IAdvancedCriterionAttributes>(attributes.content));
    }
  }

  private getCountryAdvancedFilter(attributes: IComparisonCriterionAttributes): AdvancedFilter {
    const countries = attributes?.value[attributes.filterKey].map((c: ICountry) => c.id);
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const toFilterCriterion = c => ({ legalUnit: { countryId: c } });

    return AdvancedFilterTypeMapping.toAdvancedFilter(countries, operator, toFilterCriterion, logicalOperator);
  }
}
