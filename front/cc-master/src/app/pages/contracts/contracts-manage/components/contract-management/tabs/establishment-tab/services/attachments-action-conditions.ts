import { addMonths, compareAsc, compareDesc, isAfter, isBefore, isFuture, startOfMonth, subMonths } from 'date-fns';

import { IContractEstablishment } from '../models/contract-establishment.interface';
import { IEstablishmentAttachment } from '../models/establishment-attachment.interface';

export class AttachmentLinkingConditions {
  public static minDate(establishments: IContractEstablishment[], theoreticalStartOn: string): Date {
    const allAttachments = establishments?.reduce((flattened, e) => [...flattened, ...e.contractEntities], []) ?? [];
    const lastAttachmentEndDate = getLastAttachmentEndDate(allAttachments);
    const nextMonthOfLastAttachmentEnd = !!lastAttachmentEndDate ? startOfMonth(addMonths(lastAttachmentEndDate, 1)) : null;
    const contractStartOn = !!theoreticalStartOn ? startOfMonth(new Date(theoreticalStartOn)) : null;

    return !!nextMonthOfLastAttachmentEnd ? nextMonthOfLastAttachmentEnd : contractStartOn;
  }

  public static maxDate(closeOn: string): Date {
    return !!closeOn ? startOfMonth(new Date(closeOn)) : null;
  }
}

export class AttachmentStartEditionConditions {
  public static minDate(lastCountPeriod: Date, attachments: IEstablishmentAttachment[]): Date {

    if (!lastCountPeriod && !attachments?.length) {
      return null;
    }

    const nextMonthOfLastCountPeriod = !!lastCountPeriod ? addMonths(new Date(lastCountPeriod), 1) : null;
    if (!attachments?.length) {
      return startOfMonth(nextMonthOfLastCountPeriod);
    }

    const lastAttachmentStartDate = getLastAttachmentStartDate(attachments);
    if (!nextMonthOfLastCountPeriod) {
      return startOfMonth(lastAttachmentStartDate);
    }

    return isBefore(lastAttachmentStartDate, nextMonthOfLastCountPeriod)
      ? startOfMonth(lastAttachmentStartDate)
      : startOfMonth(nextMonthOfLastCountPeriod);
  }
}

export class AttachmentEndEditionConditions {
  public static minDate(lastCountPeriod: Date, attachments: IEstablishmentAttachment[]): Date {
    if (!lastCountPeriod && !attachments?.length) {
      return null;
    }

    if (!attachments?.length) {
      return startOfMonth(lastCountPeriod);
    }

    const lastAttachmentStartDate = getLastAttachmentStartDate(attachments);
    if (!lastCountPeriod) {
      return startOfMonth(lastAttachmentStartDate);
    }

    return isAfter(lastAttachmentStartDate, lastCountPeriod)
      ? startOfMonth(lastAttachmentStartDate)
      : startOfMonth(lastCountPeriod);
  }

  public static maxDate(
    closeOn: string,
    establishmentsToEdit: IContractEstablishment[],
    attachmentsToEdit: IEstablishmentAttachment[],
  ): Date {
    const attachmentStart = getFirstUnfinishedAttachmentStartDate(establishmentsToEdit, attachmentsToEdit);
    const previousMonthOfAttachmentStart = !!attachmentStart ? subMonths(startOfMonth(attachmentStart), 1) : null;
    const closedDate = !!closeOn ? startOfMonth(new Date(closeOn)) : null;

    if (!attachmentStart && !closedDate) {
      return null;
    }

    if (!closedDate) {
      return previousMonthOfAttachmentStart;
    }

    return isAfter(previousMonthOfAttachmentStart, closedDate) ? previousMonthOfAttachmentStart : closedDate;
  }
}

const getFirstUnfinishedAttachmentStartDate = (establishments: IContractEstablishment[], attachments: IEstablishmentAttachment[]): Date => {
  const attachmentIdsToEdit = attachments?.map(a => a?.id) ?? [];
  const othersAttachments = establishments
    ?.reduce((flattened, e) => [...flattened, ...e.contractEntities], [])
    ?.filter(a => !attachmentIdsToEdit.includes(a?.id)) ?? [];

  const unfinishedAttachments = othersAttachments.filter(a => !a?.end || isFuture(new Date(a.end))) ?? [];
  const startDates = unfinishedAttachments.filter(a => !!a?.start).map(a => new Date(a.start))?.sort(compareAsc);
  return !!startDates.length ? new Date(startDates[0]) : null;
};

const getLastAttachmentStartDate = (attachments: IEstablishmentAttachment[]): Date => {
  const attachmentStartDates = attachments?.filter(a => !!a?.start).map(a => new Date(a.start)) ?? [];
  const startDates = attachmentStartDates.sort(compareDesc);
  return !!startDates?.length ? new Date(startDates[0]) : null;
};

const getLastAttachmentEndDate = (attachments: IEstablishmentAttachment[]): Date => {
  const attachmentEndDates = attachments?.filter(a => !!a?.end).map(a => new Date(a.end)) ?? [];
  const sortedDescEndDates = attachmentEndDates.sort(compareDesc);
  return !!sortedDescEndDates?.length ? sortedDescEndDates[0] : null;
};

