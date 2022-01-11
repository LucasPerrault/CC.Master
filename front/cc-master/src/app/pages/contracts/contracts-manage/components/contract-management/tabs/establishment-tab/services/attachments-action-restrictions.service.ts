import { Injectable } from '@angular/core';
import { Operation, OperationRestrictionMode, RightsService } from '@cc/aspects/rights';
import { ICount } from '@cc/domain/billing/counts';
import { isAfter, isBefore, isEqual } from 'date-fns';

import { IEstablishmentActionsContext } from '../models/establishment-actions-context.interface';
import { IEstablishmentAttachment } from '../models/establishment-attachment.interface';
import { IListEntry, ListEntryType } from '../models/establishment-list-entry.interface';
import { AttachmentsTimelineService } from './attachments-timeline.service';

@Injectable()
export class AttachmentsActionRestrictionsService {
  private readonly operationsToReadValidationContext = [Operation.ReadCounts];

  public get canReadValidationContext(): boolean {
    return this.rightsService.hasOperationsByRestrictionMode(this.operationsToReadValidationContext, OperationRestrictionMode.All);
  }

  constructor(private timelineService: AttachmentsTimelineService, private rightsService: RightsService) {}

  public canDeleteRange(attachments: IEstablishmentAttachment[], realCounts: ICount[]): boolean {
    return this.canReadValidationContext && attachments.every(ce => this.canDelete(ce, realCounts));
  }

  public canDelete(attachment: IEstablishmentAttachment, realCounts: ICount[]): boolean {
    if (!attachment || !this.canReadValidationContext) {
      return false;
    }

    const maxDate = !!attachment.end ? new Date(attachment.end) : new Date();
    return !this.hasRealCountsBetween(new Date(attachment.start), maxDate, realCounts);
  }

  public canExcludeRange(entries: IListEntry[]): boolean {
    return entries.every(e => this.canExclude(e));
  }

  public canExclude(entry: IListEntry): boolean {
    return !entry?.establishment?.contractEntities?.some(a => !a.end);
  }

  public canCancelUnlinking(entry: IListEntry, context: IEstablishmentActionsContext): boolean {
    if (!entry?.attachment?.end) {
      return false;
    }

    if (!this.canReadValidationContext) {
      return false;
    }

    if (entry.establishment.contractEntities.some(a => !a.end)) {
      return false;
    }

    if (this.hasRealCountsSince(new Date(entry.attachment.end), context.realCounts)) {
      return false;
    }

    return !context.contract.closeOn || this.timelineService.shouldBeEndedInFuture(entry?.attachment);
  }

  public canUnlinkRange(attachments: IEstablishmentAttachment[]): boolean {
    return this.canReadValidationContext && attachments.every(a => this.canUnlink(a));
  }

  public canUnlink(attachment: IEstablishmentAttachment): boolean {
    return this.canReadValidationContext && this.timelineService.isStarted(attachment) && !attachment?.end;
  }

  public canLinkRange(entries: IListEntry[]): boolean {
    return this.canReadValidationContext && entries.every(e => this.canLink(e));
  }

  public canLink(entry: IListEntry): boolean {
    if (!this.canReadValidationContext) {
        return false;
    }

    if (!!entry.establishment?.contractEntities?.length) {
      return entry.establishment?.contractEntities.every(ce => !!ce.end);
    }

    return true;
  }

  public canEditFutureStartRange(attachments: IEstablishmentAttachment[], realCounts: ICount[]): boolean {
    return this.canReadValidationContext && attachments.every(ce => this.canEditFutureStart(ce, realCounts));
  }

  public canEditFutureStart(attachment: IEstablishmentAttachment, realCounts: ICount[]): boolean {
    return this.canReadValidationContext && !!attachment && !!attachment.start && this.timelineService.isStartedInTheFuture(attachment);
  }

  public canEditEndRange(entries: IListEntry[], context: IEstablishmentActionsContext): boolean {
    return entries.every(entry => this.canEditEnd(entry, context));
  }

  public canEditEnd(entry: IListEntry, context: IEstablishmentActionsContext): boolean {
    if (!entry?.attachment?.end) {
      return false;
    }

    if (!this.canReadValidationContext) {
      return false;
    }

    if (!!context.contract?.closeOn && isEqual(new Date(entry.attachment.end), new Date(context.contract.closeOn))) {
      return false;
    }

    return !this.hasRealCountsSince(new Date(entry.attachment.end), context.realCounts);
  }

  private hasRealCountsBetween(from: Date, to: Date, realCounts: ICount[]): boolean {
    const realCountsBetween = realCounts.filter(count =>
      this.isAfterOrEquals(new Date(count.countPeriod), from)
      && this.isBeforeOrEquals(new Date(count.countPeriod), to),
    );

    return !!realCountsBetween.length;
  }

  private hasRealCountsSince(since: Date, realCounts: ICount[]): boolean {
    return this.hasRealCountsBetween(since, new Date(), realCounts);
  }

  private isBeforeOrEquals(date: Date, dateToCompare: Date): boolean {
    return isBefore(date, dateToCompare) || isEqual(date, dateToCompare);
  }

  private isAfterOrEquals(date: Date, dateToCompare: Date): boolean {
    return isAfter(date, dateToCompare) || isEqual(date, dateToCompare);
  }

  private isLinked(entry: IListEntry): boolean {
    return entry.type === ListEntryType.LinkedToThisContract || entry.type === ListEntryType.LinkedToAnotherContract;
  }
}
