import { CommonModule, CurrencyPipe, DatePipe, DecimalPipe, PercentPipe } from '@angular/common';
import { NgModule, Pipe, PipeTransform } from '@angular/core';

import { FacetType } from '../models';
import { FacetValue } from '../models/facet-value.interface';

export type FacetPipeOptions = IDatePipeOptions | IDecimalPipeOptions | IPercentPipeOptions | IStringOptions;
export interface IDatePipeOptions {
  format?: string;
  timezone?: string;
  locale?: string;
}

export interface IDecimalPipeOptions {
  digitsInfo?: string;
  locale?: string;
}

export interface IPercentPipeOptions {
  digitsInfo?: string;
  locale?: string;
}

export interface IStringOptions {
  transform?(value: string): string;
}

@Pipe({ name: 'facet' })
export class FacetPipe implements PipeTransform {

  constructor(
    private datePipe: DatePipe,
    private decimalPipe: DecimalPipe,
    private percentPipe: PercentPipe,
  ) {}

  public transform(facet: FacetValue, options?: FacetPipeOptions): string {
    if (!facet) {
      return '';
    }

    switch (facet.type) {
      case FacetType.DateTime:
        return this.transformDate(facet.value, options as IDatePipeOptions);
      case FacetType.Decimal:
      case FacetType.Integer:
        return this.transformDecimal(facet.value, options as IDecimalPipeOptions);
      case FacetType.Percentage:
        return this.transformPercent(facet.value, options as IPercentPipeOptions);
      case FacetType.String:
        return this.transformString(facet.value, options as IStringOptions);
      case FacetType.Unknown:
      default:
        return '';
    }
  }

  private transformDate(dateToString: string, options?: IDatePipeOptions): string {
    return this.datePipe.transform(dateToString, options?.format, options?.timezone, options?.locale);
  }

  private transformDecimal(decimal: number, options?: IDecimalPipeOptions): string {
    return this.decimalPipe.transform(decimal, options?.digitsInfo, options?.locale);
  }

  private transformPercent(percent: number, options?: IPercentPipeOptions): string {
    return this.percentPipe.transform(percent, options?.digitsInfo, options?.locale);
  }

  private transformString(words: string, options?: IStringOptions): string {
    return !!options?.transform ? options.transform(words) : words;
  }
}

@Pipe({ name: 'facets' })
export class FacetsPipe implements PipeTransform {

  constructor(private facetPipe: FacetPipe) {}

  public transform(facets: FacetValue[], options?: FacetPipeOptions, separator: string = ', '): string {
    return !!facets?.length ? facets?.map(facet => this.facetPipe.transform(facet, options))?.join(separator) : null;
  }
}

@NgModule({
  declarations: [FacetsPipe, FacetPipe],
  imports: [CommonModule],
  providers: [FacetPipe, DecimalPipe, CurrencyPipe, PercentPipe],
  exports: [FacetsPipe, FacetPipe],
})
export class FacetPipeModule {}
