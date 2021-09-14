import { Injectable } from '@angular/core';
import { isAfter, isBefore, isFuture, isPast, isToday } from 'date-fns';

import { IEstablishmentAttachment } from '../models/establishment-attachment.interface';

@Injectable()
export class AttachmentsTimelineService {
  public getCurrentAttachment(attachments: IEstablishmentAttachment[]): IEstablishmentAttachment {
    return attachments.find(ce => this.isStarted(ce) && !this.isFinished(ce));
  }

  public getNextAttachment(attachments: IEstablishmentAttachment[]): IEstablishmentAttachment {
    const nextAttachments = attachments.filter(ce => this.shouldBeStartedInFuture(ce) && !this.isFinished(ce));

    if (!nextAttachments.length) {
      return;
    }

    return nextAttachments.reduce((nextAttachment, attachment) =>
      !nextAttachment || isBefore(new Date(attachment.start), new Date(nextAttachment.start))
        ? attachment
        : nextAttachment,
    );
  }

  public getLastAttachment(attachments: IEstablishmentAttachment[]): IEstablishmentAttachment {
    const lastAttachments = attachments.filter(ce => this.isStarted(ce) && this.isFinished(ce));

    if (!lastAttachments.length) {
      return;
    }

    return lastAttachments.reduce((lastAttachment, attachment) =>
      !lastAttachment || isAfter(new Date(attachment.end), new Date(lastAttachment.end))
        ? attachment
        : lastAttachment,
    );
  }

  public shouldBeStartedInFuture(attachment: IEstablishmentAttachment): boolean {
    return !!attachment.start && isFuture(new Date(attachment.start));
  }

  public shouldBeEndedInFuture(attachment: IEstablishmentAttachment): boolean {
    return !!attachment.end && isFuture(new Date(attachment.end));
  }

  public isStarted(attachment: IEstablishmentAttachment): boolean {
    if (!attachment.start) {
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
