import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionResponse } from '@cc/common/queries';
import { TimelineCountsService } from '@cc/domain/billing/counts/timeline-counts-service';
import { isAfter, max } from 'date-fns';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ValidationContextStoreService } from '../../../validation-context-store.service';
import { contextAttachmentFields, IContextAttachment } from '../models/closure-form-validation-context.interface';

@Injectable()
export class CloseContractDataService {

  constructor(private httpClient: HttpClient, private contextStoreService: ValidationContextStoreService) {}

  public getLastCountPeriod$(): Observable<Date | null> {
    return this.contextStoreService.realCounts$
      .pipe(map(counts => TimelineCountsService.getLastCountPeriod(counts)));
  }

  public getMostRecentAttachment$(contractId: number): Observable<IContextAttachment | null> {
    return this.getAttachments$(contractId).pipe(
      map(as => as.reduce((mostRecent, other) => this.isMostRecent(mostRecent, other) ? mostRecent : other)));
  }

  private isMostRecent(attachment: IContextAttachment, comparison: IContextAttachment): boolean {
    const mostRecentDate = !!attachment?.end
      ? max([new Date(attachment.end), new Date(attachment.start)])
      : new Date(attachment.start);

    const mostRecentDateToCompare = !!comparison?.end
      ? max([new Date(comparison.end), new Date(comparison.start)])
      : new Date(comparison.start);

    return isAfter(mostRecentDate, mostRecentDateToCompare);
  }

  private getAttachments$(contractId: number): Observable<IContextAttachment[]> {
    const url = '/api/v3/contractentities';
    const params = new HttpParams()
      .set('fields', contextAttachmentFields)
      .set('contractId', String(contractId))
      .set('legalEntity.isActive', `${ true }`);

    return this.httpClient.get<IHttpApiV3CollectionResponse<IContextAttachment>>(url, { params })
      .pipe(map(response => response.data.items));
  }
}
