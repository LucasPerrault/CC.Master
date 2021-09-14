import { Injectable } from '@angular/core';
import { isAfter, isBefore, isEqual } from 'date-fns';

import { IContractCount } from '../models/contract-count.interface';
import { IEstablishmentAttachment } from '../models/establishment-attachment.interface';
import { AttachmentsTimelineService } from './attachments-timeline.service';

@Injectable()
export class AttachmentsActionRestrictionsService {
  constructor(private timelineService: AttachmentsTimelineService) {
  }

  public canDeleteRange(attachments: IEstablishmentAttachment[], realCounts: IContractCount[]): boolean {
    return attachments.every(ce => this.canDelete(ce, realCounts));
  }

  public canDelete(attachment: IEstablishmentAttachment, realCounts: IContractCount[]): boolean {
    if (!attachment) {
      return false;
    }

    const maxDate = !!attachment.end ? new Date(attachment.end) : new Date();
    return !this.hasRealCountsBetween(new Date(attachment.start), maxDate, realCounts);
  }

  public canCancelUnlinking(attachment: IEstablishmentAttachment, realCounts: IContractCount[]): boolean {
    if (!!attachment && !this.timelineService.shouldBeEndedInFuture(attachment)) {
      return false;
    }

    return this.canEditFutureEnd(attachment, realCounts);
  }

  public canUnlinkRange(attachments: IEstablishmentAttachment[]): boolean {
    return attachments.every(a => this.canUnlink(a));
  }

  public canUnlink(attachment: IEstablishmentAttachment): boolean {
    return this.timelineService.isStarted(attachment) && !attachment?.end;
  }

  public canLinkRange(attachments: IEstablishmentAttachment[]): boolean {
    return attachments.every(a => this.canLink(a));
  }

  public canLink(attachment: IEstablishmentAttachment): boolean {
    return !attachment || this.timelineService.isFinished(attachment);
  }

  public canEditFutureStartRange(attachments: IEstablishmentAttachment[], realCounts: IContractCount[]): boolean {
    return attachments.every(ce => this.canEditFutureStart(ce, realCounts));
  }

  public canEditFutureStart(attachment: IEstablishmentAttachment, realCounts: IContractCount[]): boolean {
    return !!attachment && !!attachment.start && this.timelineService.shouldBeStartedInFuture(attachment);
  }

  public canEditFutureEndRange(attachments: IEstablishmentAttachment[], realCounts: IContractCount[]): boolean {
    return attachments.every(ce => this.canEditFutureEnd(ce, realCounts));
  }

  public canEditFutureEnd(attachment: IEstablishmentAttachment, realCounts: IContractCount[]): boolean {
    if (!attachment || !attachment.end) {
      return false;
    }

    return !this.hasRealCountsSince(new Date(attachment.end), realCounts);
  }

  private hasRealCountsBetween(from: Date, to: Date, realCounts: IContractCount[]): boolean {
    const realCountsBetween = realCounts.filter(count =>
      this.isAfterOrEquals(new Date(count.countPeriod), from)
      && this.isBeforeOrEquals(new Date(count.countPeriod), to),
    );

    return !!realCountsBetween.length;
  }

  private hasRealCountsSince(since: Date, realCounts: IContractCount[]): boolean {
    return this.hasRealCountsBetween(since, new Date(), realCounts);
  }

  private isBeforeOrEquals(date: Date, dateToCompare: Date): boolean {
    return isBefore(date, dateToCompare) || isEqual(date, dateToCompare);
  }

  private isAfterOrEquals(date: Date, dateToCompare: Date): boolean {
    return isAfter(date, dateToCompare) || isEqual(date, dateToCompare);
  }
}
