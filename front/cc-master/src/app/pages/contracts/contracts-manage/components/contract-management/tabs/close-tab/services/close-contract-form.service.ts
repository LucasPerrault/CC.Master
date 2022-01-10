import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Operation, OperationRestrictionMode, RightsService } from '@cc/aspects/rights';
import { IHttpApiV3CollectionResponse } from '@cc/common/queries';
import { ICount } from '@cc/domain/billing/counts/count.interface';
import { isAfter, max } from 'date-fns';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ValidationContextStoreService } from '../../../validation-context-store.service';
import {
  contextAttachmentFields,
  IClosureFormValidationContext,
  IContextAttachment,
} from '../models/closure-form-validation-context.interface';

@Injectable()
export class CloseContractFormService {

  private readonly attachmentsEndpoint = '/api/v3/contractentities';
  private readonly operationsToReadValidationContext = [Operation.ReadCounts];

  public get canReadValidationContext(): boolean {
    return this.rightsService.hasOperationsByRestrictionMode(this.operationsToReadValidationContext, OperationRestrictionMode.All);
  }

  constructor(
    private httpClient: HttpClient,
    private contextStoreService: ValidationContextStoreService,
    private rightsService: RightsService,
  ) {}

  public getMinContractClosedDate(context: IClosureFormValidationContext): Date {
    if (!!context.mostRecentAttachment) {
      const mostRecentDate = context.mostRecentAttachment?.end || context.mostRecentAttachment?.start;
      return new Date(mostRecentDate);
    }

    if (!!context.lastCountPeriod) {
      return context.lastCountPeriod;
    }

    return context.theoreticalStartOn;
  }

  public getLastCountPeriod$(): Observable<Date | null> {
    return this.contextStoreService.realCounts$.pipe(
      map(counts => !!counts.length
        ? counts.reduce((a, b) => (new Date(a.countPeriod) > new Date(b.countPeriod) ? a : b))
        : null,
      ),
      map((lastCount: ICount) => lastCount?.countPeriod),
    );
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

  private getRealCounts$(): Observable<ICount[]> {
    return this.contextStoreService.realCounts$;
  }

  private getAttachments$(contractId: number): Observable<IContextAttachment[]> {
    const params = new HttpParams()
      .set('fields', contextAttachmentFields)
      .set('contractId', String(contractId))
      .set('legalEntity.isActive', `${ true }`);

    return this.httpClient.get<IHttpApiV3CollectionResponse<IContextAttachment>>(this.attachmentsEndpoint, { params })
      .pipe(map(response => response.data.items));
  }
}
