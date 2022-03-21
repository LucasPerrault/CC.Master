import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';

import { FacetScope, FacetType, IFacet } from '../../models';
import { AdvancedFilterKey } from '../criterion-formly-configuration.service';
import { FacetAdvancedFilterKey } from './facet-advanced-filter-key.enum';

@Injectable()
export class FacetFormlyConfigurationService {

  public readonly date: FormlyFieldConfig = {
    key: FacetAdvancedFilterKey.DateTime,
    type: 'date',
    templateOptions: {
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-longer is-required',
      placeholder: this.translatePipe.transform('facets_date_placeholder'),
    },
  };

  public readonly integer: FormlyFieldConfig = {
    key: FacetAdvancedFilterKey.Integer,
    type: 'input',
    templateOptions: {
      type: 'number',
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-short is-required',
    },
  };

  public readonly range: FormlyFieldConfig = {
    key: FacetAdvancedFilterKey.IntegerRange,
    type: 'range',
    templateOptions: {
      configuration: {
        range: { min: 0 },
        textfieldClass: 'palette-grey mod-outlined mod-short is-required',
        separator: 'et',
      },
      required: true,
    },
  };

  public readonly percent: FormlyFieldConfig = {
    key: FacetAdvancedFilterKey.Percent,
    type: 'input',
    templateOptions: {
      min: 0,
      max: 100,
      type: 'number',
      suffix: '%',
      required: true,
      mod: 'palette-grey mod-outlined mod-inline mod-short is-required mod-noLabel',
    },
  };

  public readonly percentRange: FormlyFieldConfig = {
    key: FacetAdvancedFilterKey.Percent,
    type: 'range',
    templateOptions: {
      configuration: {
        range: { min: 0, max: 100 },
        textfieldClass: 'palette-grey mod-outlined mod-short is-required mod-noLabel',
        separator: 'et',
      },
      suffix: '%',
      required: true,
    },
  };

  constructor(private translatePipe: TranslatePipe) {}

  public readonly facetCriterion = (facetScope: FacetScope): FormlyFieldConfig => ({
    key: AdvancedFilterKey.Criterion,
    type: 'facet-criterion',
    templateOptions: {
      required: true,
      facetScope,
      mod: 'palette-grey mod-outlined mod-inline is-required',
      placeholder: this.translatePipe.transform('facets_placeholder'),
    },
  });

  public facetStringValue = (facetScope: FacetScope, facet: IFacet): FormlyFieldConfig => {
    const filters = [`applicationId=${ facet.applicationId }`, `code=${ facet.code }`, `type=${ FacetType.String }`];
    return {
      key: FacetAdvancedFilterKey.String,
      type: 'facet-value',
      templateOptions: {
        required: true,
        facetScope,
        mod: 'palette-grey mod-outlined mod-inline mod-longer',
        filters,
        placeholder: this.translatePipe.transform('facets_string_placeholder'),
      },
    };
  };

  public facetListStringValue = (facetScope: FacetScope, facet: IFacet): FormlyFieldConfig => {
    const filters = [`applicationId=${ facet.applicationId }`, `code=${ facet.code }`, `type=${ FacetType.String }`];
    return {
      key: FacetAdvancedFilterKey.ListString,
      type: 'facet-value',
      templateOptions: {
        required: true,
        multiple: true,
        facetScope,
        mod: 'palette-grey mod-outlined mod-inline mod-longer',
        filters,
        placeholder: this.translatePipe.transform('cafe_filters_facets_string_placeholder'),
      },
    };
  };
}
