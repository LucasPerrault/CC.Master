import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionCount, IHttpApiV3CollectionCountResponse, IHttpApiV3Response } from '@cc/common/queries';
import { IPriceList } from '@cc/domain/billing/offers';
import { forkJoin, Observable, of } from 'rxjs';
import { map, switchMapTo } from 'rxjs/operators';

import { IOfferForm } from '../components/offer-form/offer-form.interface';
import { detailedOfferFields, IDetailedOffer } from '../models/detailed-offer.interface';
import { IOfferCreationDto } from '../models/offer-creation-dto.interface';
import { IOfferEditionDto } from '../models/offer-edition-dto.interface';
import { IOfferPriceListEditionDto } from '../models/offer-price-list-edition-dto.interface';
import { IPriceListsOffer, priceListsOfferFields } from '../models/price-lists-offer.interface';

@Injectable()
export class OffersDataService {
  private readonly offersEndpoint = '/api/v3/offers';
  private readonly pricesEndpoint = '/api/v3/prices/sync';

  constructor(private httpClient: HttpClient) {
  }

  public getOffers$(params: HttpParams): Observable<IHttpApiV3CollectionCount<IDetailedOffer>> {
    params = params.set('fields', detailedOfferFields);

    return this.httpClient.get<IHttpApiV3CollectionCountResponse<IDetailedOffer>>(this.offersEndpoint, { params })
      .pipe(map(res => res.data));
  }

  public getById$(offerId: number): Observable<IDetailedOffer> {
    const params = new HttpParams().set('fields', detailedOfferFields);
    const url = `${ this.offersEndpoint }/${ offerId }`;
    return this.httpClient.get<IHttpApiV3Response<IDetailedOffer>>(url, { params }).pipe(map(res => res.data));
  }

  public delete$(offerId: number): Observable<void> {
    const url = `${ this.offersEndpoint }/${ offerId }`;
    return this.httpClient.delete<void>(url);
  }

  public create$(form: IOfferForm): Observable<void> {
    const body = this.toOfferCreationDto(form);
    return this.httpClient.post<void>(this.offersEndpoint, body);
  }

  public edit$(offerId: number, form: IOfferForm): Observable<void> {
    return forkJoin([this.editOffer$(offerId, form), this.editPriceList$(offerId, form)])
      .pipe(switchMapTo(of<void>()));
  }

  public getPriceLists$(offerId: number): Observable<IPriceList[]> {
    const params = new HttpParams().set('fields', priceListsOfferFields);
    const url = `${ this.offersEndpoint }/${ offerId }`;
    return this.httpClient.get<IHttpApiV3Response<IPriceListsOffer>>(url, { params })
      .pipe(map(res => res.data.priceLists));
  }

  private toOfferCreationDto(form: IOfferForm): IOfferCreationDto {
    return {
      name: form.name,
      productId: form.product.id,
      currencyID: form.currency.code,
      tag: form.tag,
      pricingMethod: form.pricingMethod,
      forecastMethod: form.forecastMethod,
      billingMode: form.billingMode.id,
      sageBusiness: form.sageBusiness,
      unit: form.billingUnit.id,
      priceLists: form.priceLists,
    };
  }

  private editOffer$(offerId: number, form: IOfferForm): Observable<void> {
    const body = this.toOfferEditionDto(form);
    const url = `${ this.offersEndpoint }/${ offerId }`;
    return this.httpClient.put<void>(url, body);
  }

  private toOfferEditionDto(form: IOfferForm): IOfferEditionDto {
    return {
      name: form.name,
      productId: form.product.id,
      currencyID: form.currency.code,
      tag: form.tag,
      pricingMethod: form.pricingMethod,
      forecastMethod: form.forecastMethod,
      billingMode: form.billingMode.id,
      sageBusiness: form.sageBusiness,
      unit: form.billingUnit.id,
    };
  }

  private editPriceList$(offerId: number, form: IOfferForm): Observable<void> {
    const body = this.toOfferPriceListsEditionDto(offerId, form);
    const url = `${ this.pricesEndpoint }`;
    return this.httpClient.put<void>(url, body);
  }

  private toOfferPriceListsEditionDto(offerId: number, form: IOfferForm): IOfferPriceListEditionDto[] {
    return form.priceLists.map(p => this.toOfferPriceListEditionDto(offerId, p));
  }

  private toOfferPriceListEditionDto(offerId: number, priceList: IPriceList): IOfferPriceListEditionDto {
    return {
      commercialOfferID: offerId,
      fixedPrice: priceList.fixedPrice,
      unitPrice: priceList.unitPrice,
      lowerBound: priceList.lowerBound,
      upperBound: priceList.upperBound,
    };
  }
}
