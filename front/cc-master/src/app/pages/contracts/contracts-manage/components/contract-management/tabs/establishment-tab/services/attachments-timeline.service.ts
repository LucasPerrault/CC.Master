import { Injectable } from '@angular/core';
import { isFuture, isPast, isToday } from 'date-fns';

import { IEstablishmentAttachment } from '../models/establishment-attachment.interface';
import { LifecycleStep } from '../models/establishment-list-entry.interface';

@Injectable()
export class AttachmentsTimelineService {

  constructor() {}

  public getLifecycleStep(attachment: IEstablishmentAttachment): LifecycleStep {
    if (this.isInProgress(attachment)) {
      return LifecycleStep.InProgress;
    }

    if (this.isStartedInTheFuture(attachment)) {
      return LifecycleStep.StartInTheFuture;
    }

    if (this.isCompletelyFinished(attachment)) {
      return LifecycleStep.Finished;
    }

    return LifecycleStep.Unknown;
  }

  public isStartedInTheFuture(attachment: IEstablishmentAttachment): boolean {
    return !!attachment.start && isFuture(new Date(attachment.start))
      && !this.isFinished(attachment);
  }

  public isCompletelyFinished(attachment: IEstablishmentAttachment): boolean {
    return this.isStarted(attachment)
      && this.isFinished(attachment);
  }

  public isInProgress(attachment: IEstablishmentAttachment): boolean {
    return this.isStarted(attachment)
      && !this.isFinished(attachment);
  }

  public shouldBeEndedInFuture(attachment: IEstablishmentAttachment): boolean {
    return !!attachment.end && isFuture(new Date(attachment.end));
  }

  public isStarted(attachment: IEstablishmentAttachment): boolean {
    if (!attachment?.start) {
      return false;
    }

    const startDate = new Date(attachment.start);
    return isPast(startDate) || isToday(startDate);
  }

  public isFinished(attachment: IEstablishmentAttachment): boolean {
    if (!attachment.end) {
      return false;
    }

    const endDate = new Date(attachment.end);
    return isPast(endDate);
  }
}
