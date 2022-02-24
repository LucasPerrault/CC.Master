import { DatePipe } from '@angular/common';
import { Injectable } from '@angular/core';
import { Operation, OperationRestrictionMode, RightsService } from '@cc/aspects/rights';
import { TranslatePipe } from '@cc/aspects/translate';
import { isAfter } from 'date-fns';

import { ICloseContractMinDateReason } from '../models/close-contract-min-date-reason.interface';
import { IContextAttachment } from '../models/closure-form-validation-context.interface';

@Injectable()
export class CloseContractRestrictionsService {

  public get canReadValidationContext(): boolean {
    return this.rightsService.hasOperationsByRestrictionMode([Operation.ReadCounts], OperationRestrictionMode.All);
  }

  constructor(private rightsService: RightsService, private datePipe: DatePipe, private translatePipe: TranslatePipe) {}

  public getCloseMinDateReason(
    contractStartDate: Date,
    lastCountPeriod: Date,
    mostRecentAttachment: IContextAttachment,
  ): ICloseContractMinDateReason {
    const attachmentDate = this.getAttachmentDate(mostRecentAttachment);
    if (!!mostRecentAttachment && !!lastCountPeriod) {
      return isAfter(attachmentDate, lastCountPeriod)
        ? { date: attachmentDate, reason: this.getAttachmentReason(mostRecentAttachment, attachmentDate) }
        : { date: lastCountPeriod, reason: this.getLastCountReason(lastCountPeriod) };
    }

    if (!!mostRecentAttachment) {
      return { date: attachmentDate, reason: this.getAttachmentReason(mostRecentAttachment, attachmentDate) };
    }

    if (!!lastCountPeriod) {
      return { date: lastCountPeriod, reason: this.getLastCountReason(lastCountPeriod) };
    }

    return { date: contractStartDate, reason: this.getContractStartReason(contractStartDate) };
  }

  private getAttachmentReason(mostRecentAttachment: IContextAttachment, attachmentDate: Date): string {
    const translationKey = !!mostRecentAttachment?.end
      ? 'contracts_closeDate_blockingAttachment_until'
      : 'contracts_closeDate_blockingAttachment_since';

    return this.translatePipe.transform(translationKey, {
      name: mostRecentAttachment.legalEntity.name,
      id: mostRecentAttachment.legalEntity.id,
      date: this.datePipe.transform(attachmentDate, 'MMMM yyyy'),
    });
  }

  private getLastCountReason(lastCountPeriod: Date): string {
    const lastCountDate = this.datePipe.transform(lastCountPeriod, 'MMMM yyyy');
    const translationKey = 'front_contractPage_closeDateCondition_lastCountDate_callout';
    return this.translatePipe.transform(translationKey, { date: lastCountDate });
  }

  private getContractStartReason(startDate: Date): string {
    const theoreticalStartDate = this.datePipe.transform(startDate, 'MMMM yyyy');
    const translationKey = 'front_contractPage_closeDateCondition_theoreticalStartOn_callout';
    return this.translatePipe.transform(translationKey, { date: theoreticalStartDate });
  }

  private getAttachmentDate(mostRecentAttachment: IContextAttachment): Date {
    if (!mostRecentAttachment) {
      return null;
    }

    const hasEndDate = !!mostRecentAttachment?.end;
    return hasEndDate? new Date(mostRecentAttachment.end) : new Date(mostRecentAttachment.start);
  }
}
