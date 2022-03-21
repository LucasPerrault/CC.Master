import { Injectable } from '@angular/core';
import { IRange } from '@cc/common/forms/validators/range-validator';
import { ApiV3DateService } from '@cc/common/queries';

import {
  AdvancedFilter,
  AdvancedFilterOperatorMapping,
  AdvancedFilterTypeMapping,
  ComparisonOperatorDto,
  defaultEncapsulation,
  IAdvancedFilterAttributes,
  IComparisonFilterCriterionEncapsulation,
  IFacetComparisonCriterion,
  IFilterCriterionEncapsulation,
  ItemsMatchedDto,
  LogicalOperator,
} from '../../components/advanced-filter-form';
import { FacetType, IFacet } from '../../models';
import { FacetValue } from '../../models/facet-value.interface';
import { FacetAdvancedFilterKey } from './facet-advanced-filter-key.enum';

@Injectable()
export class FacetAdvancedFilterApiMappingService {
  constructor(private apiDateService: ApiV3DateService) {}

  public getAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation = defaultEncapsulation,
  ): AdvancedFilter {
    switch (attributes.filterKey) {
      case FacetAdvancedFilterKey.DateTime:
        return this.getFacetDateAdvancedFilter(attributes, encapsulate);
      case FacetAdvancedFilterKey.Decimal:
        return this.toAdvancedFilter(FacetType.Decimal, attributes, encapsulate);
      case FacetAdvancedFilterKey.DecimalRange:
        return this.toAdvancedFilterWithRange(FacetType.Decimal, attributes, encapsulate);
      case FacetAdvancedFilterKey.Integer:
        return this.toAdvancedFilter(FacetType.Integer, attributes, encapsulate);
      case FacetAdvancedFilterKey.IntegerRange:
        return this.toAdvancedFilterWithRange(FacetType.Integer, attributes, encapsulate);
      case FacetAdvancedFilterKey.String:
        return this.getFacetStringAdvancedFilter(attributes, encapsulate);
      case FacetAdvancedFilterKey.ListString:
        return this.getFacetListStringAdvancedFilter(attributes, encapsulate);
      case FacetAdvancedFilterKey.Percent:
        return this.toAdvancedFilter(FacetType.Percentage, attributes, encapsulate);
      case FacetAdvancedFilterKey.PercentRange:
        return this.toAdvancedFilterWithRange(FacetType.Percentage, attributes, encapsulate);
    }
  }

  private getFacetListStringAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation = defaultEncapsulation,
  ): AdvancedFilter {
    const facet = this.getFacet(attributes);
    const { operator, logicalOperator, itemsMatched } = AdvancedFilterOperatorMapping.getListComparisonOperatorDto(attributes.operator);
    const facetValues = attributes.value.fieldValues[attributes.filterKey] as FacetValue[];
    const values = facetValues.map(f => f?.value);
    const toFilterCriterion = this.toFacetCriterion(FacetType.String, facet, encapsulate, itemsMatched);

    return AdvancedFilterTypeMapping.toAdvancedFilter(values, operator, toFilterCriterion, logicalOperator);
  }

  private getFacetStringAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation = defaultEncapsulation,
  ): AdvancedFilter {
    const facet = this.getFacet(attributes);
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getListComparisonOperatorDto(attributes.operator);
    const facetValue = attributes.value.fieldValues[attributes.filterKey] as FacetValue;
    const value = facetValue.value;
    const toFilterCriterion = this.toFacetCriterion(FacetType.String, facet, encapsulate);

    return AdvancedFilterTypeMapping.toAdvancedFilter([value], operator, toFilterCriterion, logicalOperator);
  }

  private getFacetDateAdvancedFilter(
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation = defaultEncapsulation,
  ): AdvancedFilter {
    const facet = this.getFacet(attributes);
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const value = this.apiDateService.toApiV3DateFormat(new Date(attributes.value.fieldValues[attributes.filterKey]));
    const toFilterCriterion = this.toFacetCriterion(FacetType.DateTime, facet, encapsulate);

    return AdvancedFilterTypeMapping.toAdvancedFilter([value], operator, toFilterCriterion, logicalOperator);
  }

  private toAdvancedFilterWithRange(
    type: FacetType,
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const facet = this.getFacet(attributes);
    const logicalOperator = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator)?.logicalOperator;
    const { min, max } = attributes.value.fieldValues[attributes.filterKey] as IRange;
    const toFilterCriterion = this.toFacetCriterion(type, facet, encapsulate);

    return AdvancedFilterTypeMapping.toFilterCombination(LogicalOperator.And, [
      AdvancedFilterTypeMapping.toAdvancedFilter([min], ComparisonOperatorDto.StrictlyGreaterThan, toFilterCriterion, logicalOperator),
      AdvancedFilterTypeMapping.toAdvancedFilter([max], ComparisonOperatorDto.StrictlyLessThan, toFilterCriterion, logicalOperator),
    ]);
  }

  private toAdvancedFilter(
    type: FacetType,
    attributes: IAdvancedFilterAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const facet = this.getFacet(attributes);
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const value = attributes.value.fieldValues[attributes.filterKey];
    const toFilterCriterion = this.toFacetCriterion(type, facet, encapsulate);

    return AdvancedFilterTypeMapping.toAdvancedFilter([value], operator, toFilterCriterion, logicalOperator);
  }

  private toFacetCriterion(
    type: FacetType,
    facet: IFacet,
    encapsulate: IFilterCriterionEncapsulation,
    itemsMatched?: ItemsMatchedDto,
  ): IComparisonFilterCriterionEncapsulation {
    return ({ operator, value }) => encapsulate({
      facets: {
        value: { value, operator, type },
        identifier: { applicationId: facet?.applicationId, code: facet?.code },
        itemsMatched,
      },
    });
  }

  private getFacet(attributes: IAdvancedFilterAttributes): IFacet {
    return (attributes.criterion as IFacetComparisonCriterion).facet;
  }
}
