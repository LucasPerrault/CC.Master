import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BYPASS_INTERCEPTOR } from '@cc/aspects/errors';
import {
  ApiV3DateService,
  IHttpApiV4CollectionCountResponse,
  IHttpApiV4CollectionResponse,
} from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IOfferCreationForm } from '../components/offer-creation/offer-creation-form/offer-creation-form.interface';
import { IOfferEditionForm } from '../components/offer-edition/offer-edition-tab/offer-edition-form/offer-edition-form.interface';
import { IDetailedOffer } from '../models/detailed-offer.interface';
import { IOfferCreationDto } from '../models/offer-creation-dto.interface';
import { IOfferEditionDto } from '../models/offer-edition-dto.interface';
import { PriceListsDataService } from './price-lists-data.service';

class OfferApiEndpoint {
  public static base = '/api/commercial-offers';
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

  public getOffers$(params: HttpParams): Observable<IHttpApiV4CollectionCountResponse<IDetailedOffer>> {
    return this.httpClient.get<IHttpApiV4CollectionCountResponse<IDetailedOffer>>(OfferApiEndpoint.base, { params });
  }

  public getById$(offerId: number): Observable<IDetailedOffer> {
    const url = OfferApiEndpoint.id(offerId);
    const context = new HttpContext().set(BYPASS_INTERCEPTOR, true);
    return this.httpClient.get<IDetailedOffer>(url, { context });
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
      unit: form.billingUnit.id,
    };
  }
}
