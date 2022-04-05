import { Injectable } from '@angular/core';
import { IRange } from '@cc/common/forms/validators/range-validator';
import { ApiV3DateService } from '@cc/common/queries';

import {
  AdvancedFilter,
  AdvancedFilterOperatorMapping,
  AdvancedFilterTypeMapping,
  cast,
  ComparisonCriterion,
  ComparisonOperatorDto,
  defaultEncapsulation,
  IAdvancedCriterionAttributes,
  IComparisonCriterionAttributes,
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
    attributes: IAdvancedCriterionAttributes,
    encapsulate: IFilterCriterionEncapsulation = defaultEncapsulation,
  ): AdvancedFilter {
    const facet = this.getFacet(attributes.criterion);
    switch (facet.type) {
      case FacetType.DateTime:
        return this.getFacetDateAdvancedFilter(facet, cast<IComparisonCriterionAttributes>(attributes.content), encapsulate);
      case FacetType.Decimal:
        return this.getFacetDecimalAdvancedFilter(facet, cast<IComparisonCriterionAttributes>(attributes.content), encapsulate);
      case FacetType.Integer:
        return this.getFacetIntegerAdvancedFilter(facet, cast<IComparisonCriterionAttributes>(attributes.content), encapsulate);
      case FacetType.Percentage:
        return this.getFacetPercentageAdvancedFilter(facet, cast<IComparisonCriterionAttributes>(attributes.content), encapsulate);
      case FacetType.String:
        return this.getFacetStringAdvancedFilter(facet, cast<IComparisonCriterionAttributes>(attributes.content), encapsulate);
    }
  }

  private getFacetDateAdvancedFilter(
    facet: IFacet,
    attributes: IComparisonCriterionAttributes,
    encapsulate: IFilterCriterionEncapsulation = defaultEncapsulation,
  ): AdvancedFilter {
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const value = this.apiDateService.toApiV3DateFormat(new Date(attributes.value[attributes.filterKey]));
    const toFilterCriterion = this.toFacetCriterion(facet, encapsulate);

    return AdvancedFilterTypeMapping.toAdvancedFilter([value], operator, toFilterCriterion, logicalOperator);
  }

  private getFacetDecimalAdvancedFilter(
    facet: IFacet,
    attributes: IComparisonCriterionAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ) {
    switch (attributes.filterKey) {
      case FacetAdvancedFilterKey.Decimal:
        return this.toAdvancedFilter(facet, attributes, encapsulate);
      case FacetAdvancedFilterKey.DecimalRange:
        return this.toAdvancedFilterWithRange(facet, attributes, encapsulate);
    }
  }

  private getFacetIntegerAdvancedFilter(
    facet: IFacet,
    attributes: IComparisonCriterionAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ) {
    switch (attributes.filterKey) {
      case FacetAdvancedFilterKey.Integer:
        return this.toAdvancedFilter(facet, attributes, encapsulate);
      case FacetAdvancedFilterKey.IntegerRange:
        return this.toAdvancedFilterWithRange(facet, attributes, encapsulate);
    }
  }

  private getFacetPercentageAdvancedFilter(
    facet: IFacet,
    attributes: IComparisonCriterionAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ) {
    switch (attributes.filterKey) {
      case FacetAdvancedFilterKey.Percent:
        return this.toAdvancedFilter(facet, attributes, encapsulate);
      case FacetAdvancedFilterKey.PercentRange:
        return this.toAdvancedFilterWithRange(facet, attributes, encapsulate);
    }
  }

  private getFacetStringAdvancedFilter(
    facet: IFacet,
    attributes: IComparisonCriterionAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ) {
    switch (attributes.filterKey) {
      case FacetAdvancedFilterKey.String:
        return this.getStringAdvancedFilter(facet, attributes, encapsulate);
      case FacetAdvancedFilterKey.ListString:
        return this.getListStringAdvancedFilter(facet, attributes, encapsulate);
    }
  }

  private getListStringAdvancedFilter(
    facet: IFacet,
    attributes: IComparisonCriterionAttributes,
    encapsulate: IFilterCriterionEncapsulation = defaultEncapsulation,
  ): AdvancedFilter {
    const { operator, logicalOperator, itemsMatched } = AdvancedFilterOperatorMapping.getListComparisonOperatorDto(attributes.operator);
    const facetValues = attributes.value[attributes.filterKey] as FacetValue[];
    const values = facetValues.map(f => f?.value);
    const toFilterCriterion = this.toFacetCriterion(facet, encapsulate, itemsMatched);

    return AdvancedFilterTypeMapping.toAdvancedFilter(values, operator, toFilterCriterion, logicalOperator);
  }

  private getStringAdvancedFilter(
    facet: IFacet,
    attributes: IComparisonCriterionAttributes,
    encapsulate: IFilterCriterionEncapsulation = defaultEncapsulation,
  ): AdvancedFilter {
    const { operator, logicalOperator, itemsMatched } = AdvancedFilterOperatorMapping.getListComparisonOperatorDto(attributes.operator);
    const facetValue = attributes.value[attributes.filterKey] as FacetValue;
    const value = facetValue.value;
    const toFilterCriterion = this.toFacetCriterion(facet, encapsulate, itemsMatched);

    return AdvancedFilterTypeMapping.toAdvancedFilter([value], operator, toFilterCriterion, logicalOperator);
  }

  private toAdvancedFilterWithRange(
    facet: IFacet,
    attributes: IComparisonCriterionAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const logicalOperator = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator)?.logicalOperator;
    const { min, max } = attributes.value[attributes.filterKey] as IRange;
    const toFilterCriterion = this.toFacetCriterion(facet, encapsulate);

    return AdvancedFilterTypeMapping.toFilterCombination(LogicalOperator.And, [
      AdvancedFilterTypeMapping.toAdvancedFilter([min], ComparisonOperatorDto.StrictlyGreaterThan, toFilterCriterion, logicalOperator),
      AdvancedFilterTypeMapping.toAdvancedFilter([max], ComparisonOperatorDto.StrictlyLessThan, toFilterCriterion, logicalOperator),
    ]);
  }

  private toAdvancedFilter(
    facet: IFacet,
    attributes: IComparisonCriterionAttributes,
    encapsulate: IFilterCriterionEncapsulation,
  ): AdvancedFilter {
    const { operator, logicalOperator } = AdvancedFilterOperatorMapping.getComparisonOperatorDto(attributes.operator);
    const value = attributes.value[attributes.filterKey];
    const toFilterCriterion = this.toFacetCriterion(facet, encapsulate);

    return AdvancedFilterTypeMapping.toAdvancedFilter([value], operator, toFilterCriterion, logicalOperator);
  }

  private toFacetCriterion(
    facet: IFacet,
    encapsulate: IFilterCriterionEncapsulation,
    itemsMatched?: ItemsMatchedDto,
  ): IComparisonFilterCriterionEncapsulation {
    return ({ operator, value }) => encapsulate({
      facets: {
        value: { value, operator, type: facet.type },
        identifier: { applicationId: facet?.applicationId, code: facet?.code },
        itemsMatched,
      },
    });
  }

  private getFacet(criterion: ComparisonCriterion): IFacet {
    return (criterion as IFacetComparisonCriterion)?.facet;
  }
}
