import { Injectable } from '@angular/core';
import { IProduct, ProductsService } from '@cc/domain/billing/offers';
import { forkJoin, Observable, of } from 'rxjs';
import { map, take } from 'rxjs/operators';

import { OfferState, offerStates } from '../components/offer-filters/offer-state-filter';
import { billingModes, IBillingMode } from '../enums/billing-mode.enum';
import { currencies, IOfferCurrency } from '../models/offer-currency.interface';
import { IOfferFiltersForm } from '../models/offer-filters-form.interface';
import { IOfferQueryParams } from '../models/offer-query-params.interface';

@Injectable()
export class OffersFilterRoutingService {

  constructor(private productsService: ProductsService) { }

  public toFilters$(queryParams: IOfferQueryParams): Observable<IOfferFiltersForm> {
    return forkJoin([
      this.getProduct$(queryParams.productId),
    ]).pipe(
      map(([product]) => ({
        search: queryParams.search,
        tag: queryParams.tag,
        product,
        currencies: this.getCurrencies(queryParams.currency),
        billingModes: this.getBillingModes(queryParams.billingMode),
        state: this.getState(queryParams.state),
      }),
    ));
  }

  public toRoutingParams(filters: IOfferFiltersForm): IOfferQueryParams {
    return {
      search: this.getSafeRoutingParams(filters?.search),
      tag: this.getSafeRoutingParams(filters?.tag),
      productId: this.getSafeRoutingParams(filters?.product?.id?.toString()),
      currency: this.getSafeRoutingParams(filters?.currencies?.map(c => c?.code).join(',')),
      billingMode: this.getSafeRoutingParams(filters?.billingModes?.map(c => c?.id).join(',')),
      state: this.getSafeRoutingParams(filters.state),
    };
  }

  private getProduct$(params: string): Observable<IProduct> {
    const productIds = this.convertToNumbers(params);
    return !!productIds?.length
      ? this.productsService.getProductById$(productIds[0]).pipe(take(1))
      : of(null);
  }

  private getCurrencies(params: string): IOfferCurrency[] {
    const currencyCodes = this.convertToNumbers(params);
    return currencies.filter(c => currencyCodes.includes(c.code));
  }

  private getBillingModes(params: string): IBillingMode[] {
    const billingModeId = params?.split(',')?.map(b => b?.toLowerCase()) ?? [];
    return billingModes.filter(b => billingModeId.includes(b.id.toLowerCase()));
  }

  private getState(params: string): OfferState {
    const defaultState = OfferState.NoArchived;
    return offerStates.find(s => params?.toLowerCase() === s.id.toLowerCase())?.id ?? defaultState;
  }

  private getSafeRoutingParams(queryParams: string): string {
    return !!queryParams ? queryParams : null;
  }

  private convertToNumbers(values: string): number[] {
    return values?.split(',')?.map(idToString => parseInt(idToString, 10)) ?? [];
  }
}
