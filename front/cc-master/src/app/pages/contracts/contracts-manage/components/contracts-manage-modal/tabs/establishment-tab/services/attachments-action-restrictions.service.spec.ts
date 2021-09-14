import { createServiceFactory, SpectatorService } from '@ngneat/spectator';
import { addMonths, endOfMonth, startOfMonth, subMonths } from 'date-fns';

import { CountCode } from '../../../../../../../../domain/billing/counts/count-code.enum';
import { AttachmentEndReason } from '../constants/attachment-end-reason.const';
import { IContractCount } from '../models/contract-count.interface';
import { IEstablishmentAttachment } from '../models/establishment-attachment.interface';
import { IEstablishmentContract } from '../models/establishment-contract.interface';
import { AttachmentsActionRestrictionsService } from './attachments-action-restrictions.service';
import { AttachmentsTimelineService } from './attachments-timeline.service';

const fakeAttachment = (start: Date, end: Date): IEstablishmentAttachment => ({
  start: start?.toString(),
  end: end?.toString(),
  id: 1,
  name: 'fake',
  nbMonthFree: 0,
  endReason: AttachmentEndReason.Modification,
  legalEntityId: 1,
  contractID: 1,
  contract: {} as IEstablishmentContract,
});
const fakeRealCount = (countPeriod: Date): IContractCount => ({
  id: 1,
  countDate: addMonths(countPeriod, 1),
  countPeriod,
  code: CountCode.Count,
});

const today = new Date();

describe('AttachmentsActionRestrictionsService', () => {
  let spectator: SpectatorService<AttachmentsActionRestrictionsService>;
  const createService = createServiceFactory({
    providers: [AttachmentsTimelineService],
    service: AttachmentsActionRestrictionsService,
  });

  beforeEach(() => spectator = createService());

  it('should delete attachment', () => {
    const attachmentStart = startOfMonth(today);
    const mockedAttachment = fakeAttachment(attachmentStart, null);
    const mockedRealCounts = [fakeRealCount(subMonths(attachmentStart, 1)), fakeRealCount(subMonths(attachmentStart, 2))];

    const result = spectator.service.canDelete(mockedAttachment, mockedRealCounts);

    expect(result).toBeTruthy();
  });

  it('should not delete attachment when is null', () => {
    const start = startOfMonth(subMonths(today, 2));
    const mockedRealCounts = [fakeRealCount(start), fakeRealCount(addMonths(start, 1))];

    const result = spectator.service.canDelete(null, mockedRealCounts);

    expect(result).toBeFalsy();
  });

  it('should not delete attachment when it has real counts between its period', () => {
    const attachmentStart = startOfMonth(subMonths(today, 2));
    const mockedAttachment = fakeAttachment(attachmentStart, null);
    const mockedRealCounts = [fakeRealCount(attachmentStart), fakeRealCount(addMonths(attachmentStart, 1))];

    const result = spectator.service.canDelete(mockedAttachment, mockedRealCounts);

    expect(result).toBeFalsy();
  });

  it('should unlink attachment', () => {
    const mockedAttachment = fakeAttachment(subMonths(startOfMonth(today), 4), null);

    const result = spectator.service.canUnlink(mockedAttachment);

    expect(result).toBeTruthy();
  });

  it('should not unlink attachment when is not started', () => {
    const nextMonth = addMonths(startOfMonth(today), 1);
    const mockedFutureAttachment = fakeAttachment(nextMonth, null);

    const resultWithFutureActivation = spectator.service.canUnlink(mockedFutureAttachment);

    expect(resultWithFutureActivation).toBeFalsy();
  });

  it('should not unlink attachment when has an end date', () => {
    const lastMonth = subMonths(today, 4);
    const mockedEndedAttachment = fakeAttachment(startOfMonth(lastMonth), endOfMonth(lastMonth));

    const resultWithTerminatedAttachment = spectator.service.canUnlink(mockedEndedAttachment);

    expect(resultWithTerminatedAttachment).toBeFalsy();
  });

  it('should link attachment when is finished', () => {
    const lastMonth = subMonths(today, 1);
    const mockedEndedAttachment = fakeAttachment(startOfMonth(lastMonth), endOfMonth(lastMonth));

    const resultWithEndedAttachment = spectator.service.canLink(mockedEndedAttachment);

    expect(resultWithEndedAttachment).toBeTruthy();
  });

  it('should link attachment when is null', () => {
    const resultWithoutAttachment = spectator.service.canLink(null);

    expect(resultWithoutAttachment).toBeTruthy();
  });

  it('should not link attachment when is not finished', () => {
    const nextMonth = addMonths(today, 1);
    const mockedAttachment = fakeAttachment(startOfMonth(today), endOfMonth(nextMonth));

    const result = spectator.service.canLink(mockedAttachment);

    expect(result).toBeFalsy();
  });

  it('should edit attachment\'s start date', () => {
    const nextMonth = startOfMonth(addMonths(today, 1));
    const lastMonth = startOfMonth(subMonths(today, 1));
    const mockedRealCounts = [fakeRealCount(lastMonth), fakeRealCount(subMonths(lastMonth, 1))];
    const mockedAttachment = fakeAttachment(nextMonth, null);

    const result = spectator.service.canEditFutureStart(mockedAttachment, mockedRealCounts);

    expect(result).toBeTruthy();
  });

  it('should not edit attachment\'s start date when it not begins in the future', () => {
    const lastMonth = startOfMonth(subMonths(today, 1));
    const mockedRealCounts = [fakeRealCount(lastMonth), fakeRealCount(subMonths(lastMonth, 2))];
    const mockedStartedAttachment = fakeAttachment(lastMonth, null);

    const resultWithStartedAttachment = spectator.service.canEditFutureStart(mockedStartedAttachment, mockedRealCounts);

    expect(resultWithStartedAttachment).toBeFalsy();
  });

  it('should not edit attachment\'s start date when has no start date', () => {
    const lastMonth = startOfMonth(subMonths(today, 1));
    const mockedRealCounts = [fakeRealCount(lastMonth), fakeRealCount(subMonths(lastMonth, 2))];
    const mockedAttachment = fakeAttachment(null, null);

    const result = spectator.service.canEditFutureStart(mockedAttachment, mockedRealCounts);

    expect(result).toBeFalsy();
  });

  it('should not edit attachment\'s start date when is null', () => {
    const lastMonth = startOfMonth(subMonths(today, 1));
    const mockedRealCounts = [fakeRealCount(lastMonth), fakeRealCount(subMonths(lastMonth, 2))];

    const result = spectator.service.canEditFutureStart(null, mockedRealCounts);

    expect(result).toBeFalsy();
  });

  it('should edit attachment\'s end date', () => {
    const nextMonth = startOfMonth(addMonths(today, 1));
    const lastMonth = startOfMonth(subMonths(today, 1));
    const mockedRealCounts = [fakeRealCount(lastMonth), fakeRealCount(subMonths(lastMonth, 1))];
    const mockedAttachment = fakeAttachment(lastMonth, nextMonth);

    const result = spectator.service.canEditFutureEnd(mockedAttachment, mockedRealCounts);

    expect(result).toBeTruthy();
  });

  it('should not edit attachment\'s end date when has no end date', () => {
    const lastMonth = startOfMonth(subMonths(today, 1));
    const mockedRealCounts = [fakeRealCount(lastMonth), fakeRealCount(subMonths(lastMonth, 2))];
    const mockedStartedAttachment = fakeAttachment(lastMonth, null);

    const resultWithStartedAttachment = spectator.service.canEditFutureEnd(mockedStartedAttachment, mockedRealCounts);

    expect(resultWithStartedAttachment).toBeFalsy();
  });

  it('should not edit attachment\'s end date when is null', () => {
    const lastMonth = startOfMonth(subMonths(today, 1));
    const mockedRealCounts = [fakeRealCount(lastMonth), fakeRealCount(subMonths(lastMonth, 2))];

    const result = spectator.service.canEditFutureEnd(null, mockedRealCounts);

    expect(result).toBeFalsy();
  });

  it('should not edit attachment\'s end date when has real counts after the end date', () => {
    const start = startOfMonth(subMonths(today, 4));
    const end = endOfMonth(subMonths(today, 3));
    const mockedRealCounts = [fakeRealCount(start), fakeRealCount(addMonths(end, 1))];
    const mockedAttachment = fakeAttachment(start, end);

    const result = spectator.service.canEditFutureEnd(mockedAttachment, mockedRealCounts);

    expect(result).toBeFalsy();
  });
});
