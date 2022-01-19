import { Injectable } from '@angular/core';
import { IProduct, ProductsService } from '@cc/domain/billing/offers';
import { forkJoin, Observable, of } from 'rxjs';
import { map, take } from 'rxjs/operators';

import { OfferState, offerStates } from '../components/offer-filters/offer-state-filter';
import { billingModes, IBillingMode } from '../enums/billing-mode.enum';
import { currencies, IOfferCurrency } from '../models/offer-currency.interface';
import { IOfferFiltersForm } from '../models/offer-filters-form.interface';

export interface IOfferRoutingParams {
  search: string;
  tag: string;
  productId: string;
  currencies: string;
  billingModes: string;
  state: string;
}

@Injectable()
export class OffersFilterRoutingService {

  constructor(private productsService: ProductsService) { }

  public toFilters$(routingParams: IOfferRoutingParams): Observable<IOfferFiltersForm> {
    return forkJoin([
      this.getProduct$(routingParams.productId),
    ]).pipe(
      map(([product]) => ({
        search: routingParams.search,
        tag: this.getTag(routingParams.tag),
        product,
        currencies: this.getCurrencies(routingParams.currencies),
        billingModes: this.getBillingModes(routingParams.billingModes),
        state: this.getState(routingParams.state),
      }),
    ));
  }

  public toRoutingParams(filters: IOfferFiltersForm): IOfferRoutingParams {
    return {
      search: this.getSafeRoutingParams(filters?.search),
      tag: this.getSafeRoutingParams(filters?.tag),
      productId: this.getSafeRoutingParams(filters?.product?.id?.toString()),
      currencies: this.getSafeRoutingParams(filters?.currencies?.map(c => c?.code).join(',')),
      billingModes: this.getSafeRoutingParams(filters?.billingModes?.map(c => c?.id).join(',')),
      state: this.getSafeRoutingParams(filters.state),
    };
  }

  private getTag(params: string): string {
    // It is defined in the offer model in the back project.
    // It is used for the default selection.
    const defaultTag = 'Catalogues';
    return !!params ? params : defaultTag;
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
