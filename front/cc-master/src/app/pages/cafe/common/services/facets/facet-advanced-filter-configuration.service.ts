import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

import { ComparisonOperator, ICriterionConfiguration } from '../../components/advanced-filter-form';
import { FacetScope, FacetType } from '../../models';
import { FacetAdvancedFilterKey } from './facet-advanced-filter-key.enum';
import { FacetFormlyConfigurationService } from './facet-formly-configuration.service';

@Injectable()
export class FacetAdvancedFilterConfigurationService {

  constructor(
    private translatePipe: TranslatePipe,
    private formlyConfiguration: FacetFormlyConfigurationService,
  ) {}

  public criterions = (facetScope: FacetScope): ICriterionConfiguration[] => ([
    {
      key: FacetType.DateTime,
      name: FacetType.DateTime,
      operators: [
        { id: ComparisonOperator.StrictlyGreaterThan, name: this.translatePipe.transform('cafe_filters_operator_since') },
        { id: ComparisonOperator.StrictlyLessThan, name: this.translatePipe.transform('cafe_filters_operator_until') },
      ],
      componentConfigs: [
        {
          key: FacetAdvancedFilterKey.DateTime,
          components: [this.formlyConfiguration.date],
        },
      ],
    },
    {
      key: FacetType.Integer,
      name: FacetType.Integer,
      operators: [
        { id: ComparisonOperator.Equals, name: this.translatePipe.transform('cafe_filters_operator_equals_number') },
        { id: ComparisonOperator.NotEquals, name: this.translatePipe.transform('cafe_filters_operator_notEqual_number') },
        { id: ComparisonOperator.StrictlyGreaterThan, name: this.translatePipe.transform('cafe_filters_operator_greater_than') },
        { id: ComparisonOperator.StrictlyLessThan, name: this.translatePipe.transform('cafe_filters_operator_less_than') },
        { id: ComparisonOperator.Between, name: this.translatePipe.transform('cafe_filters_operator_between') },
      ],
      componentConfigs: [
        {
          key: FacetAdvancedFilterKey.Integer,
          components: [this.formlyConfiguration.integer],
          matchingOperators: [
            ComparisonOperator.Equals,
            ComparisonOperator.NotEquals,
            ComparisonOperator.StrictlyGreaterThan,
            ComparisonOperator.StrictlyLessThan,
          ],
        },
        {
          key: FacetAdvancedFilterKey.Between,
          components: [this.formlyConfiguration.range],
          matchingOperators: [
            ComparisonOperator.Between,
          ],
        },
      ],
    },
    {
      key: FacetType.Decimal,
      name: FacetType.Decimal,
      operators: [
        { id: ComparisonOperator.Equals, name: this.translatePipe.transform('cafe_filters_operator_equals_number') },
        { id: ComparisonOperator.NotEquals, name: this.translatePipe.transform('cafe_filters_operator_notEqual_number') },
        { id: ComparisonOperator.StrictlyGreaterThan, name: this.translatePipe.transform('cafe_filters_operator_greater_than') },
        { id: ComparisonOperator.StrictlyLessThan, name: this.translatePipe.transform('cafe_filters_operator_less_than') },
        { id: ComparisonOperator.Between, name: this.translatePipe.transform('cafe_filters_operator_between') },
      ],
      componentConfigs: [
        {
          key: FacetAdvancedFilterKey.Decimal,
          components: [this.formlyConfiguration.integer],
          matchingOperators: [
            ComparisonOperator.Equals,
            ComparisonOperator.NotEquals,
            ComparisonOperator.StrictlyGreaterThan,
            ComparisonOperator.StrictlyLessThan,
          ],
        },
        {
          key: FacetAdvancedFilterKey.Between,
          components: [this.formlyConfiguration.range],
          matchingOperators: [
            ComparisonOperator.Between,
          ],
        },
      ],
    },
    {
      key: FacetType.Percentage,
      name: FacetType.Percentage,
      operators: [
        { id: ComparisonOperator.Equals, name: this.translatePipe.transform('cafe_filters_operator_equals_number') },
        { id: ComparisonOperator.NotEquals, name: this.translatePipe.transform('cafe_filters_operator_notEqual_number') },
        { id: ComparisonOperator.StrictlyGreaterThan, name: this.translatePipe.transform('cafe_filters_operator_greater_than') },
        { id: ComparisonOperator.StrictlyLessThan, name: this.translatePipe.transform('cafe_filters_operator_less_than') },
        { id: ComparisonOperator.Between, name: this.translatePipe.transform('cafe_filters_operator_between') },
      ],
      componentConfigs: [
        {
          key: FacetAdvancedFilterKey.Percent,
          components: [this.formlyConfiguration.percent],
          matchingOperators: [
            ComparisonOperator.Equals,
            ComparisonOperator.NotEquals,
            ComparisonOperator.StrictlyGreaterThan,
            ComparisonOperator.StrictlyLessThan,
          ],
        },
        {
          key: FacetAdvancedFilterKey.Between,
          components: [this.formlyConfiguration.percentRange],
          matchingOperators: [
            ComparisonOperator.Between,
          ],
        },
      ],
    },
    {
      key: FacetType.String,
      name: FacetType.String,
      operators: [
        { id: ComparisonOperator.ListAreAmong, name: this.translatePipe.transform('cafe_filters_operator_areAmong') },
        { id: ComparisonOperator.ListNotContains, name: this.translatePipe.transform('cafe_filters_operator_areNotAmong') },
        { id: ComparisonOperator.ListContainsOnly, name: this.translatePipe.transform('cafe_filters_operator_containsOnly') },
        { id: ComparisonOperator.ListContains, name: this.translatePipe.transform('cafe_filters_operator_contains') },
      ],
      componentConfigs: [
        {
          key: FacetAdvancedFilterKey.String,
          components: [this.formlyConfiguration.facetStringValues(facetScope)],
          matchingOperators: [
            ComparisonOperator.ListAreAmong,
            ComparisonOperator.ListNotContains,
            ComparisonOperator.ListContains,
          ],
        },
        {
          key: FacetAdvancedFilterKey.String,
          components: [this.formlyConfiguration.facetStringValue(facetScope)],
          matchingOperators: [ComparisonOperator.ListContainsOnly],
        },
      ],
    },
  ]);

  public criterionFormlyFieldConfigs = (facetScope: FacetScope): FormlyFieldConfig[] => ([
    this.formlyConfiguration.facetCriterion(facetScope),
  ]);
}
