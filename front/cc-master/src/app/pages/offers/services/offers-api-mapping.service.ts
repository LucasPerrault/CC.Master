import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiSortHelper } from '@cc/common/queries';
import { ISortParams } from '@cc/common/sort';
import { IProduct } from '@cc/domain/billing/offers';

import { IBillingMode } from '../enums/billing-mode.enum';
import { IOfferCurrency } from '../models/offer-currency.interface';
import { IOfferFiltersForm } from '../models/offer-filters-form.interface';

interface HttpParamsAttributes {
  filters: IOfferFiltersForm;
  sort: ISortParams;
}

enum OfferQueryParamKey {
  Name = 'name',
  Tag = 'tag',
  ProductId = 'product.id',
  Currency = 'currencyId',
  BillingMode = 'billingMode',
}

@Injectable()
export class OffersApiMappingService {
  public toHttpParams(attributes: HttpParamsAttributes): HttpParams {
    let params = new HttpParams();
    params = this.setSortParams(params, attributes.sort);
    return this.setOfferFilters(params, attributes.filters);
  }

  private setSortParams(params: HttpParams, sortParams: ISortParams): HttpParams {
    if (!sortParams) {
      return params;
    }

    return ApiSortHelper.toV4HttpParams(params, sortParams);
  }

  private setOfferFilters(params: HttpParams, filters: IOfferFiltersForm): HttpParams {
    if (!filters) {
      return params;
    }

    params = this.getNameHttpParams(params, filters.search);
    params = this.getTagHttpParams(params, filters.tag);
    params = this.getProductHttpParams(params, filters.product);
    params = this.getCurrencyHttpParams(params, filters.currencies);
    params = this.getBillingModeHttpParams(params, filters.billingModes);
    return params;
  }

  private getNameHttpParams(params: HttpParams, search: string): HttpParams {
    return !!search
      ? params.set(OfferQueryParamKey.Name, `like,${ encodeURIComponent(search) }`)
      : params.delete(OfferQueryParamKey.Name);
  }

  private getTagHttpParams(params: HttpParams, tag: string): HttpParams {
    return !!tag
      ? params.set(OfferQueryParamKey.Tag, tag)
      : params.delete(OfferQueryParamKey.Tag);
  }

  private getProductHttpParams(params: HttpParams, product: IProduct): HttpParams {
    return !!product
      ? params.set(OfferQueryParamKey.ProductId, product.id)
      : params.delete(OfferQueryParamKey.ProductId);
  }

  private getCurrencyHttpParams(params: HttpParams, currencies: IOfferCurrency[]): HttpParams {
    return !!currencies?.length
      ? params.set(OfferQueryParamKey.Currency, currencies.map(c => c.code).join(','))
      : params.delete(OfferQueryParamKey.Currency);
  }

  private getBillingModeHttpParams(params: HttpParams, billingModes: IBillingMode[]): HttpParams {
    return !!billingModes?.length
      ? params.set(OfferQueryParamKey.BillingMode, billingModes.map(b => b.id).join(','))
      : params.delete(OfferQueryParamKey.BillingMode);
  }

}
