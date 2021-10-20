import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiV3DateService, IHttpApiV3CollectionResponse } from '@cc/common/queries';
import { forkJoin, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { AttachmentEndReason } from '../constants/attachment-end-reason.const';
import { contractEstablishmentFields, IContractEstablishment } from '../models/contract-establishment.interface';

@Injectable()
export class EstablishmentsDataService {
  private readonly establishmentsEndpoint = `/api/v3/legalEntities`;
  private readonly attachmentsEndpoint = `/api/v3/contractEntities`;
  private readonly excludedEntitiesEndpoint = `/api/v3/excludedEntities`;

  constructor(private httpClient: HttpClient, private apiV3DateService: ApiV3DateService) {}

  public getEstablishments$(environmentId: number): Observable<IContractEstablishment[]> {
    const params = new HttpParams()
      .set('fields', contractEstablishmentFields)
      .set('isActive', `${ true }`)
      .set('environmentId', `${ environmentId }`);

    return this.httpClient.get<IHttpApiV3CollectionResponse<IContractEstablishment>>(this.establishmentsEndpoint, { params })
      .pipe(map(response => response.data.items));
  }

  public createAttachmentRange$(contractId: number, establishmentIds: number[], start: Date, nbMonthFree: number): Observable<void> {
    const request$ = establishmentIds.map(establishmentId => this.createAttachment$(contractId, establishmentId, start, nbMonthFree));
    return forkJoin(...request$);
  }

  public createAttachment$(contractId: number, establishmentId: number, start: Date, nbMonthFree: number): Observable<void> {
    return this.httpClient.post<void>(this.attachmentsEndpoint, {
      contractId,
      start,
      nbMonthFree,
      legalEntityId: establishmentId,
    });
  }

  public deleteAttachmentRange$(attachmentIds: number[]): Observable<void> {
    const requests$ = attachmentIds.map(attachmentId => this.deleteAttachment$(attachmentId));
    return forkJoin(...requests$);
  }

  public deleteAttachment$(attachmentId: number): Observable<void> {
    return this.httpClient.delete<void>(`${ this.attachmentsEndpoint }/${ attachmentId }`);
  }

  public updateAttachmentStartRange$(attachmentIds: number[], start: Date, nbMonthFree: AttachmentEndReason): Observable<void> {
    const request$ = attachmentIds.map(attachmentId => this.updateAttachmentStart$(attachmentId, start, nbMonthFree));
    return forkJoin(...request$);
  }

  public updateAttachmentStart$(attachmentId: number, start: Date, nbMonthFree: AttachmentEndReason): Observable<void> {
    return this.httpClient.put<void>(`${ this.attachmentsEndpoint }/${ attachmentId }`, {
      start: this.apiV3DateService.toApiV3DateFormat(start),
      nbMonthFree,
    });
  }

  public updateAttachmentEndRange$(attachmentIds: number[], end: Date, endReason: AttachmentEndReason): Observable<void> {
    const request$ = attachmentIds.map(attachmentId => this.updateAttachmentEnd$(attachmentId, end, endReason));
    return forkJoin(...request$);
  }


  public updateAttachmentEnd$(attachmentId: number, end: Date, endReason: AttachmentEndReason): Observable<void> {
    return this.httpClient.put<void>(`${ this.attachmentsEndpoint }/${ attachmentId }`, {
      end: this.apiV3DateService.toApiV3DateFormat(end),
      endReason,
    });
  }

  public cancelAttachmentUnlinkingRange$(attachmentIds: number[]): Observable<void> {
    const requests$ = attachmentIds.map(attachmentId => this.cancelAttachmentUnlinking$(attachmentId));
    return forkJoin(...requests$);
  }

  public cancelAttachmentUnlinking$(attachmentId: number): Observable<void> {
    return this.httpClient.put<void>(`${ this.attachmentsEndpoint }/${ attachmentId }`, {
      end: null,
      endReason: null,
    });
  }

  public excludeEstablishmentRange$(establishmentIds: number[], productId: number): Observable<void> {
    const request$ = establishmentIds.map(establishmentId => this.excludeEstablishment$(establishmentId, productId));
    return forkJoin(...request$);
  }

  public excludeEstablishment$(establishmentId: number, productId: number): Observable<void> {
    return this.httpClient.post<void>(this.excludedEntitiesEndpoint, {
      legalEntityId: establishmentId,
      productId,
    });
  }

  public synchronize(environmentId: number): Observable<void> {
    return this.httpClient.get<void>(`${ this.establishmentsEndpoint }/update?environmentId=${ environmentId }`);
  }
}
