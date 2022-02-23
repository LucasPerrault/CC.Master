import { CommonModule, DatePipe, DecimalPipe, PercentPipe, registerLocaleData } from '@angular/common';
import localeFr from '@angular/common/locales/fr';
import { LOCALE_ID } from '@angular/core';
import { inject, TestBed } from '@angular/core/testing';

import { FacetType } from '../models';
import { FacetValue } from '../models/facet-value.interface';
import { FacetPipe } from './facet.pipe';

const fakeFacetValue = (type: FacetType, value: any): FacetValue => ({
  id: 1,
  type,
  value,
} as FacetValue);


describe('FacetPipe', () => {
  let facetPipe: FacetPipe;
  const localeId = 'fr-FR';

  registerLocaleData(localeFr, localeId);


  beforeEach(() => TestBed.configureTestingModule({
    imports: [CommonModule],
    providers: [FacetPipe, DatePipe, DecimalPipe, PercentPipe, {
      provide: LOCALE_ID,
      useValue: localeId,
    }],
  }));

  beforeEach(inject([FacetPipe], p => facetPipe = p));

  it('should transform string facet value', () => {
    const expected = 'Iam a string facet';
    const result = facetPipe.transform(fakeFacetValue(FacetType.String, expected));

    expect(result).toEqual(expected);
  });

  it('should transform string facet value with options', () => {
    const value = 'Iam a string facet';
    const options = { transform: (v: string) => v + ' with options.' };
    const result = facetPipe.transform(fakeFacetValue(FacetType.String, value), options);

    const expected = value + ' with options.';
    expect(result).toEqual(expected);
  });

  it('should transform datetime facet value', () => {
    const datetime =  '2018-07-31';
    const result = facetPipe.transform(fakeFacetValue(FacetType.DateTime, datetime));

    const expected = '31 juil. 2018';
    expect(result).toEqual(expected);
  });

  it('should transform datetime facet value with options', () => {
    const datetime =  '2018-07-31';
    const options = { format: 'shortDate' };
    const result = facetPipe.transform(fakeFacetValue(FacetType.DateTime, datetime), options);

    const expected = '31/07/2018';
    expect(result).toEqual(expected);
  });

  it('should transform integer facet value', () => {
    const integer =  2;
    const result = facetPipe.transform(fakeFacetValue(FacetType.Integer, integer));

    const expected = '2';
    expect(result).toEqual(expected);
  });

  it('should transform decimal facet value', () => {
    const decimal =  2.06;
    const result = facetPipe.transform(fakeFacetValue(FacetType.Decimal, decimal));

    const expected = '2,06';
    expect(result).toEqual(expected);
  });

  it('should transform decimal facet value with options', () => {
    const decimal =  2.06;
    const options = { digitsInfo: '1.0-1' };
    const result = facetPipe.transform(fakeFacetValue(FacetType.Decimal, decimal), options);

    const expected = '2,1';
    expect(result).toEqual(expected);
  });

  it('should transform percent facet value', () => {
    const percent =  0.2;
    const result = facetPipe.transform(fakeFacetValue(FacetType.Percentage, percent));

    const expected = '20 %';
    expect(result).toEqual(expected);
  });

  it('should transform percent facet value with options', () => {
    const percent =  0.207678;
    const options = { digitsInfo: '1.0-2' };
    const result = facetPipe.transform(fakeFacetValue(FacetType.Percentage, percent), options);

    const expected = '20,77 %';
    expect(result).toEqual(expected);
  });

  it('should transform unknown facet value', () => {
    const result = facetPipe.transform(fakeFacetValue(FacetType.Unknown, null));

    expect(result).toEqual('');
  });
});
