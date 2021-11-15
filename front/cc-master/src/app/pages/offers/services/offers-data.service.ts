import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiV3DateService, IHttpApiV3CollectionCount, IHttpApiV3CollectionCountResponse, IHttpApiV3Response } from '@cc/common/queries';
import { IOffer, IPriceList, offerFields } from '@cc/domain/billing/offers';
import { forkJoin, Observable, of } from 'rxjs';
import { map, switchMapTo } from 'rxjs/operators';

import { IOfferCreationForm } from '../components/offer-creation/offer-creation-form/offer-creation-form.interface';
import { IOfferEditionForm } from '../components/offer-edition/offer-edition-form/offer-edition-form.interface';
import { detailedOfferFields, IDetailedOffer } from '../models/detailed-offer.interface';
import { IOfferCreationDto } from '../models/offer-creation-dto.interface';
import { IOfferEditionDto } from '../models/offer-edition-dto.interface';
import { IPriceListCreationDto } from '../models/price-list-creation-dto.interface';
import { IPriceListEditionDto, IPriceRowEditionDto } from '../models/price-list-edition-dto.interface';
import { IPriceListForm, IPriceRowForm } from '../models/price-list-form.interface';
import { IPriceListsOffer, priceListsOfferFields } from '../models/price-lists-offer.interface';

@Injectable()
export class OffersDataService {
  private readonly offersEndpoint = '/api/v3/offers';

  constructor(private httpClient: HttpClient, private apiDateService: ApiV3DateService) {
  }

  public getOffers$(params: HttpParams): Observable<IHttpApiV3CollectionCount<IDetailedOffer>> {
    params = params.set('fields', detailedOfferFields);

    return this.httpClient.get<IHttpApiV3CollectionCountResponse<IDetailedOffer>>(this.offersEndpoint, { params })
      .pipe(map(res => res.data));
  }

  public getName$(offerId: number): Observable<string> {
    const params = new HttpParams().set('fields', offerFields);
    const url = `${ this.offersEndpoint }/${ offerId }`;
    return this.httpClient.get<IHttpApiV3Response<IOffer>>(url, { params }).pipe(map(res => res.data.name));
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

  public create$(form: IOfferCreationForm): Observable<void> {
    const body = this.toOfferCreationDto(form);
    return this.httpClient.post<void>(this.offersEndpoint, body);
  }

  public edit$(offerId: number, priceListId: number, form: IOfferEditionForm): Observable<void> {
    const requests$ = [this.editOffer$(offerId, form), this.editPriceList$(offerId, priceListId, form.priceList)];
    return forkJoin(requests$).pipe(switchMapTo(of<void>()));
  }

  public getPriceLists$(offerId: number): Observable<IPriceList[]> {
    const params = new HttpParams().set('fields', priceListsOfferFields);
    const url = `${ this.offersEndpoint }/${ offerId }`;
    return this.httpClient.get<IHttpApiV3Response<IPriceListsOffer>>(url, { params })
      .pipe(map(res => res.data.priceLists));
  }

  private toOfferCreationDto(form: IOfferCreationForm): IOfferCreationDto {
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
      priceLists: [this.toPriceListDto(form.priceList)],
    };
  }

  private toPriceListDto(priceList: IPriceListForm): IPriceListCreationDto {
    return {
      startsOn: this.apiDateService.toApiV3DateFormat(priceList.startsOn),
      rows: priceList.rows,
    };
  }

  private editOffer$(offerId: number, form: IOfferEditionForm): Observable<void> {
    const body = this.toOfferEditionDto(form);
    const url = `${ this.offersEndpoint }/${ offerId }`;
    return this.httpClient.put<void>(url, body);
  }

  private toOfferEditionDto(form: IOfferEditionForm): IOfferEditionDto {
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

  private editPriceList$(offerId: number, priceListId: number, form: IPriceListForm): Observable<void> {
    const body = [this.toPriceListEditionDto(priceListId, form)];
    const url = `/api/v3/offers/${ offerId }/updatePriceList`;
    return this.httpClient.put<void>(url, body);
  }

  private toPriceListEditionDto(id: number, form: IPriceListForm): IPriceListEditionDto {
    const startsOn = this.apiDateService.toApiV3DateFormat(form.startsOn);
    const rows = form.rows?.map(row => this.toPriceRowEditionDto(id, row));
    return { id, startsOn, rows };
  }

  private toPriceRowEditionDto(listId: number, form: IPriceRowForm): IPriceRowEditionDto {
    return { ...form, listId };
  }
}
