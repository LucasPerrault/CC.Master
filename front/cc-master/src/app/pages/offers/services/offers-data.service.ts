import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BYPASS_INTERCEPTOR } from '@cc/aspects/errors';
import {
  ApiV3DateService,
  IHttpApiV4CollectionCountResponse,
} from '@cc/common/queries';
import { IPriceList } from '@cc/domain/billing/offers';
import { Observable, of } from 'rxjs';

import { IOfferCreationForm } from '../components/offer-creation/offer-creation-form/offer-creation-form.interface';
import { IOfferEditionForm } from '../components/offer-edition/offer-edition-tab/offer-edition-form/offer-edition-form.interface';
import { IDetailedOffer, IDetailedOfferWithoutUsage } from '../models/detailed-offer.interface';
import { IOfferCreationDto } from '../models/offer-creation-dto.interface';
import { IOfferEditionDto } from '../models/offer-edition-dto.interface';
import { IOfferUsage } from '../models/offer-usage.interface';
import { IPriceListEditionDto } from '../models/price-list-edition-dto.interface';
import { PriceListsDataService } from './price-lists-data.service';

class OfferApiEndpoint {
  public static base = '/api/commercial-offers';
  public static usages = `${ OfferApiEndpoint.base }/usages`;
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

  public getOffersWithoutUsage$(params: HttpParams): Observable<IHttpApiV4CollectionCountResponse<IDetailedOfferWithoutUsage>> {
    params = params.set('fields.root', 'count');
    return this.httpClient.get<IHttpApiV4CollectionCountResponse<IDetailedOfferWithoutUsage>>(OfferApiEndpoint.base, { params });
  }

  public getById$(offerId: number): Observable<IDetailedOfferWithoutUsage> {
    const url = OfferApiEndpoint.id(offerId);
    const context = new HttpContext().set(BYPASS_INTERCEPTOR, true);
    return this.httpClient.get<IDetailedOffer>(url, { context });
  }

  public getUsages$(offerIds: number[]): Observable<IOfferUsage[]> {
    if (!offerIds?.length) {
      return of([]);
    }

    const params = new HttpParams().set('offerId', offerIds.join(','));
    return this.httpClient.get<IOfferUsage[]>(OfferApiEndpoint.usages, { params });
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

  public edit$(offer: IDetailedOffer, form: IOfferEditionForm): Observable<void> {
    const url = OfferApiEndpoint.id(offer.id);
    const body = this.toEditionDto(offer, form);
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

  private toEditionDto(offerToEdit: IDetailedOffer, form: IOfferEditionForm): IOfferEditionDto {
    const id = offerToEdit.id;
    const isArchived = offerToEdit.isArchived;

    return {
      id,
      name: form.name,
      productId: form.product.id,
      currencyID: form.currency.code,
      tag: form.tag,
      pricingMethod: form.pricingMethod,
      forecastMethod: form.forecastMethod,
      billingMode: form.billingMode.id,
      unit: form.billingUnit.id,
      isArchived,
    };
  }
}
