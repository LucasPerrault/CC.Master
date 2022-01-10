import { createServiceFactory, SpectatorService } from '@ngneat/spectator';
import { addYears, endOfMonth, startOfMonth, subMonths, subYears } from 'date-fns';

import { AttachmentEndReason } from '../constants/attachment-end-reason.const';
import { IContractEstablishment } from '../models/contract-establishment.interface';
import { IEstablishmentAttachment } from '../models/establishment-attachment.interface';
import { IEstablishmentContract } from '../models/establishment-contract.interface';
import { IEstablishmentExcludedEntity } from '../models/establishment-excluded-entity.interface';
import { LifecycleStep } from '../models/establishment-list-entry.interface';
import { AttachmentsTimelineService } from './attachments-timeline.service';
import { EstablishmentsTimelineService } from './establishments-timeline.service';

const fakeEstablishment = (
  excludedEntities: IEstablishmentExcludedEntity[] = [],
  attachments: IEstablishmentAttachment[],
  isActive: boolean = true,
): IContractEstablishment => ({
  id: 1,
  isActive,
  name: 'fake-establishment',
  excludedEntities,
  contractEntities: attachments,
});

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
const lastMonth = subMonths(today, 1);

describe('EstablishmentsTimelineService', () => {
  let spectator: SpectatorService<EstablishmentsTimelineService>;
  const createService = createServiceFactory({
    service: EstablishmentsTimelineService,
    providers: [AttachmentsTimelineService],
  });

  beforeEach(() => spectator = createService());

  it('should get inactive lifecycle step', () => {
    const currentAttachment = fakeAttachment(startOfMonth(lastMonth), null);
    const establishment = fakeEstablishment([], [currentAttachment], false);

    const result = spectator.service.getLifecycleStepError(establishment);

    expect(result).toEqual(LifecycleStep.Inactive);
  });

  it('should get waiting first activation lifecycle step', () => {
    const establishment = fakeEstablishment([], []);

    const result = spectator.service.getLifecycleStepError(establishment);

    expect(result).toEqual(LifecycleStep.WaitingFirstActivation);
  });

  it('should get not linked since a date lifecycle step', () => {
    const pastAttachment = fakeAttachment(startOfMonth(subMonths(today, 4)), endOfMonth(subMonths(today, 3)));
    const establishment = fakeEstablishment([], [pastAttachment]);

    const result = spectator.service.getLifecycleStepError(establishment);

    expect(result).toEqual(LifecycleStep.NotLinkedSince);
  });

  it('should get not linked between past and future', () => {
    const pastAttachment = fakeAttachment(startOfMonth(subMonths(today, 4)), endOfMonth(subMonths(today, 3)));
    const futureAttachment = fakeAttachment(startOfMonth(addYears(today, 4)), null);
    const establishment = fakeEstablishment([], [pastAttachment, futureAttachment]);

    const result = spectator.service.getLifecycleStepError(establishment);

    expect(result).toEqual(LifecycleStep.NotLinkedBetweenPastAndFuture);
  });

  it('should get if it is currently covered', () => {
    const pastAttachment = fakeAttachment(startOfMonth(subMonths(today, 4)), endOfMonth(subMonths(today, 3)));
    const currentAttachment = fakeAttachment(startOfMonth(lastMonth), null);
    const futureAttachment = fakeAttachment(startOfMonth(addYears(today, 4)), null);

    const noCurrentlyCovered = fakeEstablishment([], [pastAttachment, futureAttachment]);
    const currentlyCovered = fakeEstablishment([], [pastAttachment, currentAttachment, futureAttachment]);

    const falsyResult = spectator.service.isCurrentlyCovered(noCurrentlyCovered);
    const truthyResult = spectator.service.isCurrentlyCovered(currentlyCovered);

    expect(falsyResult).toBeFalsy();
    expect(truthyResult).toBeTruthy();
  });

  it('should get only the last attachment before today', () => {
    const oldestAttachment = fakeAttachment(startOfMonth(subYears(today, 4)), endOfMonth(subYears(today, 3)));
    const lastAttachmentBeforeToday = fakeAttachment(startOfMonth(subMonths(today, 4)), endOfMonth(subMonths(today, 3)));

    const establishment = fakeEstablishment([], [lastAttachmentBeforeToday, oldestAttachment]);

    const result = spectator.service.getLastAttachmentBeforeToday(establishment);

    expect(result).toEqual(lastAttachmentBeforeToday);
  });

  it('should get only the next attachment after today', () => {
    const youngestAttachment = fakeAttachment(startOfMonth(addYears(today, 10)), endOfMonth(addYears(today, 3)));
    const nextAttachmentAfterToday = fakeAttachment(startOfMonth(addYears(today, 4)), null);

    const establishment = fakeEstablishment([], [youngestAttachment, nextAttachmentAfterToday]);

    const result = spectator.service.getNextAttachmentAfterToday(establishment);

    expect(result).toEqual(nextAttachmentAfterToday);
  });

});
