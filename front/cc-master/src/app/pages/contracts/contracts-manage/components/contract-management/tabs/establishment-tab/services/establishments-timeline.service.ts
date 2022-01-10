import { Injectable } from '@angular/core';
import { isAfter, isBefore } from 'date-fns';

import { IContractEstablishment } from '../models/contract-establishment.interface';
import { IEstablishmentAttachment } from '../models/establishment-attachment.interface';
import { LifecycleStep } from '../models/establishment-list-entry.interface';
import { AttachmentsTimelineService } from './attachments-timeline.service';

@Injectable()
export class EstablishmentsTimelineService {
  private readonly errorLifecycleSteps = [
    LifecycleStep.Inactive,
    LifecycleStep.WaitingFirstActivation,
    LifecycleStep.NotLinkedSince,
    LifecycleStep.NotLinkedBetweenPastAndFuture,
  ];

  constructor(private attachmentsTimeline: AttachmentsTimelineService) {}

  public getLifecycleStepError(establishment: IContractEstablishment): LifecycleStep | null {
    const isCurrentlyCovered = this.isCurrentlyCovered(establishment);
    if (!establishment.isActive && isCurrentlyCovered) {
      return LifecycleStep.Inactive;
    }

    if (!establishment.contractEntities?.length) {
      return LifecycleStep.WaitingFirstActivation;
    }

    const willBeCovered = !!this.getNextAttachmentAfterToday(establishment)?.start;
    const wasCovered = !!this.getLastAttachmentBeforeToday(establishment)?.end;

    if (wasCovered && !willBeCovered && !isCurrentlyCovered) {
      return LifecycleStep.NotLinkedSince;
    }

    if (wasCovered && willBeCovered && !isCurrentlyCovered) {
      return LifecycleStep.NotLinkedBetweenPastAndFuture;
    }
  }

  public isErrorLifecycleStep(lifecycleStep: LifecycleStep): boolean {
    return this.errorLifecycleSteps.includes(lifecycleStep);
  }

  public isCurrentlyCovered(establishment: IContractEstablishment): boolean {
    return establishment.contractEntities.some(a => this.attachmentsTimeline.isInProgress(a));
  }

  public getNextAttachmentAfterToday(establishment: IContractEstablishment): IEstablishmentAttachment {
    const futureAttachments = establishment.contractEntities.filter(a => this.attachmentsTimeline.isStartedInTheFuture(a));
    if (!futureAttachments.length) {
      return null;
    }

    return futureAttachments.reduce((nextAttachment, attachment) =>
      !nextAttachment || isBefore(new Date(attachment.start), new Date(nextAttachment.start)) ? attachment : nextAttachment);
  }

  public getLastAttachmentBeforeToday(establishment: IContractEstablishment): IEstablishmentAttachment {
    const pastAttachments = establishment.contractEntities.filter(a => this.attachmentsTimeline.isCompletelyFinished(a));
    if (!pastAttachments.length) {
      return null;
    }

    return pastAttachments.reduce((lastAttachment, attachment) =>
      !lastAttachment || isAfter(new Date(attachment.end), new Date(lastAttachment.end)) ? attachment : lastAttachment);
  }
}
