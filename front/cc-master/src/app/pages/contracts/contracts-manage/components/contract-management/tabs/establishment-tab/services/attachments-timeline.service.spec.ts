import { createServiceFactory, SpectatorService } from '@ngneat/spectator';
import { addMonths, addYears, endOfMonth, startOfMonth, subMonths } from 'date-fns';

import { AttachmentEndReason } from '../constants/attachment-end-reason.const';
import { IEstablishmentAttachment } from '../models/establishment-attachment.interface';
import { IEstablishmentContract } from '../models/establishment-contract.interface';
import { LifecycleStep } from '../models/establishment-list-entry.interface';
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
const futureAttachment = fakeAttachment(startOfMonth(addYears(today, 4)), null);
const finishedAttachment = fakeAttachment(startOfMonth(subMonths(today, 4)), endOfMonth(subMonths(today, 3)));

describe('AttachmentsTimelineService', () => {
  let spectator: SpectatorService<AttachmentsTimelineService>;
  const createService = createServiceFactory({
    service: AttachmentsTimelineService,
  });

  beforeEach(() => spectator = createService());

  it('should get in progress lifecycle step', () => {
    const result = spectator.service.getLifecycleStep(currentAttachment);

    expect(result).toEqual(LifecycleStep.InProgress);
  });

  it('should get started in future lifecycle step', () => {
    const result = spectator.service.getLifecycleStep(futureAttachment);

    expect(result).toEqual(LifecycleStep.StartInTheFuture);
  });

  it('should get unknown lifecycle step', () => {
    const unknownAttachment = fakeAttachment(null, null);

    const result = spectator.service.getLifecycleStep(unknownAttachment);

    expect(result).toEqual(LifecycleStep.Unknown);
  });

  it('should be started in future', () => {
    const result = spectator.service.isStartedInTheFuture(futureAttachment);
    expect(result).toBeTruthy();
  });

  it('should be in progress ', () => {
    const result = spectator.service.isInProgress(currentAttachment);
    expect(result).toBeTruthy();
  });

  it('should be completely finished ', () => {
    const result = spectator.service.isFinished(finishedAttachment);
    expect(result).toBeTruthy();
  });

  it('should be ended in future', () => {
    const result = spectator.service.shouldBeEndedInFuture(futureDeactivatedAttachment);
    expect(result).toBeTruthy();
  });

  it('should be finished', () => {
    const result = spectator.service.isFinished(finishedAttachment);
    expect(result).toBeTruthy();
  });

  it('should be started', () => {
    const result = spectator.service.isStarted(currentAttachment);
    expect(result).toBeTruthy();
  });
});
