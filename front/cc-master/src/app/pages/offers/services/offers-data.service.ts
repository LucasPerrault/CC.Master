import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiV3DateService, IHttpApiV3CollectionCount, IHttpApiV3CollectionCountResponse, IHttpApiV3Response } from '@cc/common/queries';
import { IOffer, IPriceList, offerFields } from '@cc/domain/billing/offers';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IOfferCreationForm } from '../components/offer-creation/offer-creation-form/offer-creation-form.interface';
import { IOfferEditionForm } from '../components/offer-edition/offer-edition-tab/offer-edition-form/offer-edition-form.interface';
import { detailedOfferFields, IDetailedOffer } from '../models/detailed-offer.interface';
import { IOfferCreationDto } from '../models/offer-creation-dto.interface';
import { IOfferEditionDto } from '../models/offer-edition-dto.interface';
import { IPriceListsOffer, priceListsOfferFields } from '../models/price-lists-offer.interface';
import { PriceListsDataService } from './price-lists-data.service';

class OfferApiEndpoint {
  public static base = '/api/v3/offers';
  public static id = (offerId: number) => `${ OfferApiEndpoint.base }/${ offerId }`;
}

@Injectable()
export class OffersDataService {
  constructor(
    private httpClient: HttpClient,
    private apiDateService: ApiV3DateService,
    private listsDataService: PriceListsDataService,
  ) {
  }

  public getOffers$(params: HttpParams): Observable<IHttpApiV3CollectionCount<IDetailedOffer>> {
    params = params.set('fields', detailedOfferFields);

    return this.httpClient.get<IHttpApiV3CollectionCountResponse<IDetailedOffer>>(OfferApiEndpoint.base, { params })
      .pipe(map(res => res.data));
  }

  public getName$(offerId: number): Observable<string> {
    const url = OfferApiEndpoint.id(offerId);
    const params = new HttpParams().set('fields', offerFields);
    return this.httpClient.get<IHttpApiV3Response<IOffer>>(url, { params }).pipe(map(res => res.data.name));
  }

  public getById$(offerId: number): Observable<IDetailedOffer> {
    const url = OfferApiEndpoint.id(offerId);
    const params = new HttpParams().set('fields', detailedOfferFields);
    return this.httpClient.get<IHttpApiV3Response<IDetailedOffer>>(url, { params }).pipe(map(res => res.data));
  }

  public getPriceLists$(offerId: number): Observable<IPriceList[]> {
    const url = OfferApiEndpoint.id(offerId);
    const params = new HttpParams().set('fields', priceListsOfferFields);
    return this.httpClient.get<IHttpApiV3Response<IPriceListsOffer>>(url, { params })
      .pipe(map(res => res.data.priceLists));
  }

  public delete$(offerId: number): Observable<void> {
    const url = OfferApiEndpoint.id(offerId);
    return this.httpClient.delete<void>(url);
  }

  public create$(form: IOfferCreationForm): Observable<void> {
    const url = OfferApiEndpoint.base;
    const body = this.toCreationDto(form);
    return this.httpClient.post<void>(url, body);
  }

  public edit$(offerId: number, form: IOfferEditionForm): Observable<void> {
    const url = OfferApiEndpoint.id(offerId);
    const body = this.toEditionDto(form);
    return this.httpClient.put<void>(url, body);
  }

  private toCreationDto(form: IOfferCreationForm): IOfferCreationDto {
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
      priceLists: [this.listsDataService.toCreationDto(form.priceList)],
    };
  }

  private toEditionDto(form: IOfferEditionForm): IOfferEditionDto {
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
}
