import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BYPASS_INTERCEPTOR } from '@cc/aspects/errors';
import { DownloadService } from '@cc/common/download';
import {
  ApiV3DateService,
  IHttpApiV4CollectionCountResponse, IHttpApiV4CollectionResponse,
} from '@cc/common/queries';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';

import { IOfferCreationForm } from '../components/offer-creation/offer-creation-form/offer-creation-form.interface';
import { IOfferEditionForm } from '../components/offer-edition/offer-edition-tab/offer-edition-form/offer-edition-form.interface';
import { IUploadedOffer } from '../components/offer-import/uploaded-offer-dto.interface';
import { IDetailedOffer, IDetailedOfferWithoutUsage } from '../models/detailed-offer.interface';
import { IOfferCreationDto } from '../models/offer-creation-dto.interface';
import { IOfferEditionDto } from '../models/offer-edition-dto.interface';
import { IOfferUsage } from '../models/offer-usage.interface';
import { PriceListsDataService } from './price-lists-data.service';

class OfferApiEndpoint {
  public static base = '/api/commercial-offers';
  public static usages = `${ OfferApiEndpoint.base }/usages`;
  public static createRange = `${ OfferApiEndpoint.base }/creation`;
  public static upload = `${ OfferApiEndpoint.base }/upload-csv`;
  public static download = `${ OfferApiEndpoint.base }/upload-csv/template`;
  public static id = (offerId: number) => `${ OfferApiEndpoint.base }/${ offerId }`;
}

@Injectable()
export class OffersDataService {
  constructor(
    private httpClient: HttpClient,
    private apiDateService: ApiV3DateService,
    private listsDataService: PriceListsDataService,
    private downloadService: DownloadService,
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

  public createRange$(form: IUploadedOffer[]): Observable<void> {
    const url = OfferApiEndpoint.base;
    const body = this.toMultipleCreationDto(form);
    return this.httpClient.post<void>(url, body);
  }

  public edit$(offer: IDetailedOffer, form: IOfferEditionForm): Observable<void> {
    const url = OfferApiEndpoint.id(offer.id);
    const body = this.toEditionDto(offer, form);
    return this.httpClient.put<void>(url, body);
  }

  public upload$(file: any): Observable<IUploadedOffer[]> {
    const url = OfferApiEndpoint.upload;
    const formData = new FormData();
    formData.append('file', file);
    return this.httpClient.post<IHttpApiV4CollectionResponse<IUploadedOffer>>(url, formData)
      .pipe(map(res => res.items));
  }

  public download$(): Observable<void> {
    const url = OfferApiEndpoint.download;
    return this.downloadService.download$(url);
  }

  private toMultipleCreationDto(uploadedOffers: IUploadedOffer[]): IOfferCreationDto[] {
    return uploadedOffers.map(o => ({
      name: o.name,
      productId: o.product.id,
      currencyID: o.currencyID,
      tag: o.category,
      pricingMethod: o.pricingMethod,
      forecastMethod: o.forecastMethod,
      billingMode: o.billingMode,
      unit: o.billingUnit,
      sageBusiness: '',
      priceLists: o.priceLists.map(priceList => this.listsDataService.toCreationDto(priceList)),
    }));
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
