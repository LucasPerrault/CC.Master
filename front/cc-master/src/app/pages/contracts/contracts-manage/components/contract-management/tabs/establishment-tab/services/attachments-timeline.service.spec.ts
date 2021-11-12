import { createServiceFactory, SpectatorService } from '@ngneat/spectator';
import { addMonths, addYears, endOfMonth, startOfMonth, subMonths } from 'date-fns';

import { AttachmentEndReason } from '../constants/attachment-end-reason.const';
import { IEstablishmentAttachment } from '../models/establishment-attachment.interface';
import { IEstablishmentContract } from '../models/establishment-contract.interface';
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

const today = new Date();
const nextMonth = addMonths(today, 1);
const lastMonth = subMonths(today, 1);

const currentAttachment = fakeAttachment(startOfMonth(lastMonth), null);
const futureDeactivatedAttachment = fakeAttachment(startOfMonth(lastMonth), endOfMonth(nextMonth));
const futureAttachmentClosestToday = fakeAttachment(startOfMonth(nextMonth), null);
const futureAttachment = fakeAttachment(startOfMonth(addYears(today, 4)), null);
const terminatedAttachmentClosestToday = fakeAttachment(startOfMonth(subMonths(today, 2)), endOfMonth(lastMonth));
const terminatedAttachment = fakeAttachment(startOfMonth(subMonths(today, 4)), endOfMonth(subMonths(today, 3)));

describe('AttachmentsTimelineService', () => {
  let spectator: SpectatorService<AttachmentsTimelineService>;
  const createService = createServiceFactory({
    service: AttachmentsTimelineService,
  });

  beforeEach(() => spectator = createService());

  it('should output the current attachment', () => {
    const mockedAttachments = [futureAttachment, currentAttachment, terminatedAttachment];
    const result = spectator.service.getCurrentAttachment(mockedAttachments);
    expect(result).toBe(currentAttachment);
  });

  it('should output the next attachment', () => {
    const mockedAttachments = [futureAttachment, futureAttachmentClosestToday, currentAttachment, terminatedAttachment];
    const result = spectator.service.getNextAttachment(mockedAttachments);
    expect(result).toBe(futureAttachmentClosestToday);
  });

  it('should output the last attachment', () => {
    const mockedAttachments = [terminatedAttachment, terminatedAttachmentClosestToday, currentAttachment, futureAttachment];
    const result = spectator.service.getLastAttachment(mockedAttachments);
    expect(result).toBe(terminatedAttachmentClosestToday);
  });

  it('should be started in future', () => {
    const result = spectator.service.shouldBeStartedInFuture(futureAttachment);
    expect(result).toBeTruthy();
  });

  it('should be ended in future', () => {
    const result = spectator.service.shouldBeEndedInFuture(futureDeactivatedAttachment);
    expect(result).toBeTruthy();
  });

  it('should be finished', () => {
    const result = spectator.service.isFinished(terminatedAttachment);
    expect(result).toBeTruthy();
  });

  it('should be started', () => {
    const result = spectator.service.isStarted(currentAttachment);
    expect(result).toBeTruthy();
  });
});
