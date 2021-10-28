import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionResponse } from '@cc/common/queries';
import { CountsService } from '@cc/domain/billing/counts';
import { ICount } from '@cc/domain/billing/counts/count.interface';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { attachmentEndedFields, IAttachmentEnded } from '../models/attachment-ended.interface';
import { IClosureFormValidationContext } from '../models/closure-form-validation-context.interface';

@Injectable()
export class CloseContractFormService {

  private readonly attachmentsEndpoint = '/api/v3/contractentities';

  constructor(private httpClient: HttpClient, private countsService: CountsService) {}

  public getMaxContractClosedDate(context: IClosureFormValidationContext): Date {
    if (!!context.lastAttachmentEndedDate) {
      return context.lastAttachmentEndedDate;
    }

    if (!!context.lastCountPeriod) {
      return context.lastCountPeriod;
    }

    return context.theoreticalStartOn;
  }

  public getLastAttachmentEnded$(contractId: number): Observable<IAttachmentEnded | null> {
    return this.getAttachmentsEnded$(contractId).pipe(
      map(entities => !!entities.length
        ? entities.reduce((a, b) => (new Date(a.end) > new Date(b.end) ? a : b))
        : null,
      ));
  }

  public getLastCountPeriod$(contractId: number): Observable<Date | null> {
    return this.countsService.getRealCounts$(contractId).pipe(
      map(counts => !!counts.length
        ? counts.reduce((a, b) => (new Date(a.countPeriod) > new Date(b.countPeriod) ? a : b))
        : null,
      ),
      map((lastCount: ICount) => lastCount?.countPeriod),
    );
  }

  private getAttachmentsEnded$(contractId: number): Observable<IAttachmentEnded[]> {
    const params = new HttpParams()
      .set('fields', attachmentEndedFields)
      .set('contractId', String(contractId))
      .set('isActive', `${ true }`)
      .set('end', 'notequal,null');

    return this.httpClient.get<IHttpApiV3CollectionResponse<IAttachmentEnded>>(this.attachmentsEndpoint, { params })
      .pipe(map(response => response.data.items));
  }
}
