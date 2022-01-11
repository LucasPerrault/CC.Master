import { RightsService } from '@cc/aspects/rights';
import { ICount } from '@cc/domain/billing/counts';
import { createServiceFactory, mockProvider, SpectatorService } from '@ngneat/spectator';
import { addMonths, addYears, endOfMonth, startOfMonth, subMonths, subYears } from 'date-fns';
import { BehaviorSubject } from 'rxjs';

import { AttachmentEndReason } from '../constants/attachment-end-reason.const';
import { IContractEstablishment } from '../models/contract-establishment.interface';
import { IEstablishmentActionsContext } from '../models/establishment-actions-context.interface';
import { IEstablishmentAttachment } from '../models/establishment-attachment.interface';
import { IEstablishmentContract } from '../models/establishment-contract.interface';
import { IListEntry, ListEntryType } from '../models/establishment-list-entry.interface';
import { AttachmentsActionRestrictionsService } from './attachments-action-restrictions.service';
import { AttachmentsTimelineService } from './attachments-timeline.service';

const fakeEntry = (type: ListEntryType, establishment: IContractEstablishment, attachment: IEstablishmentAttachment) => ({
  type, establishment, attachment,
} as IListEntry);

const fakeEstablishment = (id: number, contractEntities: IEstablishmentAttachment[]): IContractEstablishment => ({
  id,
  name: 'fake-establishment',
  contractEntities,
  excludedEntities: [],
  isActive: true,
});
const fakeContract = (id: number, productId: number, closeOn: Date): IEstablishmentContract => ({
  id,
  closeOn: closeOn?.toString(),
  productId,
  name: 'fake-contract',
} as IEstablishmentContract);
const fakeContext = (
  contract: IEstablishmentContract,
  realCounts: ICount[],
  lastCountPeriod?: Date,
  allEntries: IListEntry[] = [],
): IEstablishmentActionsContext => ({
  contract,
  realCounts,
  lastCountPeriod,
  allEntries,
});
const fakeAttachment = (start: Date, end: Date, id: number = 1, contractID: number = 1): IEstablishmentAttachment => ({
  start: start?.toString(),
  end: end?.toString(),
  id,
  name: 'fake',
  nbMonthFree: 0,
  endReason: AttachmentEndReason.Modification,
  legalEntityId: 1,
  contractID,
  contract: {} as IEstablishmentContract,
});
const fakeRealCount = (countPeriod: Date): ICount => ({
  id: 1,
  countDate: addMonths(countPeriod, 1),
  countPeriod,
});

const today = new Date();
const canReadValidationContext$ = new BehaviorSubject(true);

describe('AttachmentsActionRestrictionsService', () => {
  let spectator: SpectatorService<AttachmentsActionRestrictionsService>;
  const createService = createServiceFactory({
    providers: [AttachmentsTimelineService, mockProvider(RightsService, {
      hasOperationsByRestrictionMode: () => canReadValidationContext$.value,
    })],
    service: AttachmentsActionRestrictionsService,
  });

  beforeEach(() => {
    spectator = createService();
    canReadValidationContext$.next(true);
  });

  /* canDelete() */
  it('should delete attachment', () => {
    const attachmentStart = startOfMonth(today);
    const mockedAttachment = fakeAttachment(attachmentStart, null);
    const mockedRealCounts = [fakeRealCount(subMonths(attachmentStart, 1)), fakeRealCount(subMonths(attachmentStart, 2))];

    const result = spectator.service.canDelete(mockedAttachment, mockedRealCounts);
    expect(result).toBeTruthy();

    canReadValidationContext$.next(false);
    const resultWithoutRights = spectator.service.canDelete(mockedAttachment, mockedRealCounts);
    expect(resultWithoutRights).toBeFalsy();
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

  /* canUnlink() */
  it('should unlink attachment', () => {
    canReadValidationContext$.next(true);
    const mockedAttachment = fakeAttachment(subMonths(startOfMonth(today), 4), null);

    const result = spectator.service.canUnlink(mockedAttachment);
    expect(result).toBeTruthy();

    canReadValidationContext$.next(false);
    const resultWithoutRights = spectator.service.canUnlink(mockedAttachment);
    expect(resultWithoutRights).toBeFalsy();
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

  /* canCancelUnlinking */
  it('should cancel unlinking attachment with no closed contract date', () => {
    const contract = fakeContract(1, 1, null);
    const mockedContext = fakeContext(contract, []);

    const lastMonth = startOfMonth(subMonths(today, 1));
    const attachmentToEdit = fakeAttachment(subYears(lastMonth, 2), subYears(lastMonth, 1));
    const mockedEstablishment = fakeEstablishment(1, [attachmentToEdit]);
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, mockedEstablishment, attachmentToEdit);

    const resultWithStartedAttachment = spectator.service.canCancelUnlinking(mockedEntry, mockedContext);

    expect(resultWithStartedAttachment).toBeTruthy();

    canReadValidationContext$.next(false);
    const resultWithoutRights = spectator.service.canCancelUnlinking(mockedEntry, mockedContext);
    expect(resultWithoutRights).toBeFalsy();
  });

  it('should cancel unlinking attachment with future end date but closed contract', () => {
    const lastMonth = startOfMonth(subMonths(today, 1));
    const closedContract = fakeContract(1, 1, endOfMonth(lastMonth));
    const mockedContext = fakeContext(closedContract, []);

    const attachmentToEdit = fakeAttachment(subYears(lastMonth, 2), addYears(lastMonth, 1));
    const mockedEstablishment = fakeEstablishment(1, [attachmentToEdit]);
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, mockedEstablishment, attachmentToEdit);

    const resultWithStartedAttachment = spectator.service.canCancelUnlinking(mockedEntry, mockedContext);

    expect(resultWithStartedAttachment).toBeTruthy();

    canReadValidationContext$.next(false);
    const resultWithoutRights = spectator.service.canCancelUnlinking(mockedEntry, mockedContext);
    expect(resultWithoutRights).toBeFalsy();
  });

  it('should not cancel unlinking attachment when has no end date', () => {
    const lastMonth = startOfMonth(subMonths(today, 1));
    const contract = fakeContract(1, 1, null);
    const realCounts = [fakeRealCount(lastMonth), fakeRealCount(subMonths(lastMonth, 2))];
    const mockedContext = fakeContext(contract, realCounts);
    const attachmentToEdit = fakeAttachment(subYears(lastMonth, 2), null);
    const mockedEstablishment = fakeEstablishment(1, [attachmentToEdit]);
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, mockedEstablishment, attachmentToEdit);

    const resultWithStartedAttachment = spectator.service.canCancelUnlinking(mockedEntry, mockedContext);

    expect(resultWithStartedAttachment).toBeFalsy();
  });

  it('should not cancel unlinking attachment when another attachment has no end date', () => {
    const lastMonth = startOfMonth(subMonths(today, 1));
    const contract = fakeContract(1, 1, null);
    const realCounts = [fakeRealCount(lastMonth), fakeRealCount(subMonths(lastMonth, 2))];
    const mockedContext = fakeContext(contract, realCounts);
    const futureAttachmentWithNoEnd = fakeAttachment(subYears(lastMonth, 1), null);
    const attachmentToEdit = fakeAttachment(subYears(lastMonth, 2), subYears(lastMonth, 1));
    const mockedEstablishment = fakeEstablishment(1, [attachmentToEdit, futureAttachmentWithNoEnd]);
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, mockedEstablishment, attachmentToEdit);

    const resultWithStartedAttachment = spectator.service.canCancelUnlinking(mockedEntry, mockedContext);

    expect(resultWithStartedAttachment).toBeFalsy();
  });

  it('should not cancel unlinking attachment when has real counts after the end date', () => {
    const start = startOfMonth(subMonths(today, 4));
    const end = endOfMonth(subMonths(today, 3));
    const contract = fakeContract(1, 1, null);
    const realCounts = [fakeRealCount(start), fakeRealCount(addMonths(end, 1))];
    const mockedContext = fakeContext(contract, realCounts);

    const attachmentToEdit = fakeAttachment(subYears(start, 2), end, 1, contract.id);
    const mockedEstablishment = fakeEstablishment(1, [attachmentToEdit]);
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, mockedEstablishment, attachmentToEdit);

    const result = spectator.service.canCancelUnlinking(mockedEntry, mockedContext);

    expect(result).toBeFalsy();
  });

  it('should not cancel unlinking attachment when contract is closed and attachment does not ended in the future ', () => {
    const lastMonth = startOfMonth(subMonths(today, 1));
    const closedContract = fakeContract(1, 1, lastMonth);
    const mockedContext = fakeContext(closedContract, []);
    const attachmentToEdit = fakeAttachment(subYears(lastMonth, 2), subYears(lastMonth, 1));
    const mockedEstablishment = fakeEstablishment(1, [attachmentToEdit]);
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, mockedEstablishment, attachmentToEdit);

    const resultWithStartedAttachment = spectator.service.canCancelUnlinking(mockedEntry, mockedContext);

    expect(resultWithStartedAttachment).toBeFalsy();
  });

  /* canLink() */
  it('should link establishment when should be ended in future', () => {
    const nextMonth = addMonths(today, 1);
    const mockedFutureEndedAttachment = fakeAttachment(startOfMonth(nextMonth), endOfMonth(nextMonth));
    const mockedEstablishment = fakeEstablishment(1, [mockedFutureEndedAttachment]);
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, mockedEstablishment, mockedFutureEndedAttachment);

    const resultWithEndedAttachment = spectator.service.canLink(mockedEntry);
    expect(resultWithEndedAttachment).toBeTruthy();

    canReadValidationContext$.next(false);
    const resultWithoutRights = spectator.service.canLink(mockedEntry);
    expect(resultWithoutRights).toBeFalsy();
  });

  it('should link establishment when its attachment is finished', () => {
    const lastMonth = subMonths(today, 1);
    const mockedEndedAttachment = fakeAttachment(startOfMonth(lastMonth), endOfMonth(lastMonth));
    const mockedEstablishment = fakeEstablishment(1, [mockedEndedAttachment]);
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, mockedEstablishment, mockedEndedAttachment);

    const resultWithEndedAttachment = spectator.service.canLink(mockedEntry);

    expect(resultWithEndedAttachment).toBeTruthy();
  });

  it('should link establishment when it does not have attachment', () => {
    const mockedEstablishment = fakeEstablishment(1, []);
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, mockedEstablishment, null);

    const resultWithoutAttachment = spectator.service.canLink(mockedEntry);

    expect(resultWithoutAttachment).toBeTruthy();
  });

  it('should not link establishment when is already linked to this contract', () => {
    const mockedAttachment = fakeAttachment(startOfMonth(today), null);
    const mockedEstablishment = fakeEstablishment(1, [mockedAttachment]);
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, mockedEstablishment, mockedAttachment);

    const result = spectator.service.canLink(mockedEntry);

    expect(result).toBeFalsy();
  });

  it('should not link establishment when is already linked to another contract', () => {
    const lastMonth = subMonths(today, 1);
    const contractId = 1;
    const mockedEndedAttachment = fakeAttachment(startOfMonth(lastMonth), endOfMonth(lastMonth), 1, contractId);
    const anotherContractId = 2;
    const mockedAttachment = fakeAttachment(startOfMonth(today), null, 2, anotherContractId);
    const mockedEstablishment = fakeEstablishment(1, [mockedEndedAttachment, mockedAttachment]);
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, mockedEstablishment, mockedAttachment);

    const result = spectator.service.canLink(mockedEntry);

    expect(result).toBeFalsy();
  });

  /* canEditFutureStart */
  it('should edit attachment\'s start date', () => {
    canReadValidationContext$.next(true);
    const nextMonth = startOfMonth(addMonths(today, 1));
    const lastMonth = startOfMonth(subMonths(today, 1));
    const mockedRealCounts = [fakeRealCount(lastMonth), fakeRealCount(subMonths(lastMonth, 1))];
    const mockedAttachment = fakeAttachment(nextMonth, null);

    const result = spectator.service.canEditFutureStart(mockedAttachment, mockedRealCounts);
    expect(result).toBeTruthy();

    canReadValidationContext$.next(false);
    const resultWithoutRights = spectator.service.canEditFutureStart(mockedAttachment, mockedRealCounts);
    expect(resultWithoutRights).toBeFalsy();
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

  /* canEditEnd() */
  it('should edit attachment\'s end date', () => {
    canReadValidationContext$.next(true);
    const nextMonth = startOfMonth(addMonths(today, 1));
    const lastMonth = startOfMonth(subMonths(today, 1));
    const contract = fakeContract(1, 1, null);
    const realCounts = [fakeRealCount(lastMonth), fakeRealCount(subMonths(lastMonth, 1))];
    const mockedContext = fakeContext(contract, realCounts);

    const attachmentToEdit = fakeAttachment(subYears(nextMonth, 2), nextMonth);
    const anotherAttachment = fakeAttachment(subYears(lastMonth, 2), subYears(nextMonth, 2));
    const mockedEstablishment = fakeEstablishment(1, [attachmentToEdit, anotherAttachment]);
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, mockedEstablishment, attachmentToEdit);

    const result = spectator.service.canEditEnd(mockedEntry, mockedContext);
    expect(result).toBeTruthy();

    canReadValidationContext$.next(false);
    const resultWithoutRights = spectator.service.canEditEnd(mockedEntry, mockedContext);
    expect(resultWithoutRights).toBeFalsy();
  });

  it('should not edit attachment\'s end date when has no end date', () => {
    const lastMonth = startOfMonth(subMonths(today, 1));
    const contract = fakeContract(1, 1, null);
    const realCounts = [fakeRealCount(lastMonth), fakeRealCount(subMonths(lastMonth, 2))];
    const mockedContext = fakeContext(contract, realCounts);
    const attachmentToEdit = fakeAttachment(subYears(lastMonth, 2), null);
    const mockedEstablishment = fakeEstablishment(1, [attachmentToEdit]);
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, mockedEstablishment, attachmentToEdit);

    const resultWithStartedAttachment = spectator.service.canEditEnd(mockedEntry, mockedContext);

    expect(resultWithStartedAttachment).toBeFalsy();
  });

  it('should not edit attachment\'s end date when is null', () => {
    const lastMonth = startOfMonth(subMonths(today, 1));
    const contract = fakeContract(1, 1, null);
    const realCounts = [fakeRealCount(lastMonth), fakeRealCount(subMonths(lastMonth, 2))];
    const mockedContext = fakeContext(contract, realCounts);
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, null, null);

    const result = spectator.service.canEditEnd(mockedEntry, mockedContext);

    expect(result).toBeFalsy();
  });

  it('should not edit attachment\'s end date when has real counts after the end date', () => {
    const start = startOfMonth(subMonths(today, 4));
    const end = endOfMonth(subMonths(today, 3));
    const contract = fakeContract(1, 1, null);
    const realCounts = [fakeRealCount(start), fakeRealCount(addMonths(end, 1))];
    const mockedContext = fakeContext(contract, realCounts);

    const attachmentToEdit = fakeAttachment(subYears(start, 2), end, 1, contract.id);
    const mockedEstablishment = fakeEstablishment(1, [attachmentToEdit]);
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, mockedEstablishment, attachmentToEdit);

    const result = spectator.service.canEditEnd(mockedEntry, mockedContext);

    expect(result).toBeFalsy();
  });

  it('should not edit attachment\'s end date when current end equals closed contract date', () => {
    const start = startOfMonth(subMonths(today, 4));
    const end = endOfMonth(subMonths(today, 3));

    const closedContract = fakeContract(1, 1, end);
    const mockedContext = fakeContext(closedContract, []);

    const attachmentToEdit = fakeAttachment(subYears(start, 2), end, 1, closedContract.id);
    const mockedEstablishment = fakeEstablishment(1, [attachmentToEdit]);
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, mockedEstablishment, attachmentToEdit);

    const result = spectator.service.canEditEnd(mockedEntry, mockedContext);

    expect(result).toBeFalsy();
  });

  /* canExclude() */
  it('should exclude establishment when it has attachment with end', () => {
    const start = startOfMonth(subMonths(Date.now(), 2));
    const mockedAttachmentWithEnd = fakeAttachment(start, endOfMonth(addMonths(start, 2)));
    const mockedEstablishment = fakeEstablishment(1, [mockedAttachmentWithEnd]);
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, mockedEstablishment, mockedAttachmentWithEnd);

    const result = spectator.service.canExclude(mockedEntry);

    expect(result).toBeTruthy();
  });

  it('should exclude establishment when it has no attachment', () => {
    const mockedEstablishment = fakeEstablishment(1, []);
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, mockedEstablishment, null);

    const result = spectator.service.canExclude(mockedEntry);

    expect(result).toBeTruthy();
  });

  it('should exclude establishment when is null', () => {
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, null, null);

    const result = spectator.service.canExclude(mockedEntry);

    expect(result).toBeTruthy();
  });

  it('should not exclude establishment when it has attachments without end', () => {
    const mockedAttachment = fakeAttachment(subYears(startOfMonth(subMonths(Date.now(), 2)), 2), null);
    const mockedEstablishment = fakeEstablishment(1, [mockedAttachment]);
    const mockedEntry = fakeEntry(ListEntryType.LinkedToThisContract, mockedEstablishment, mockedAttachment);

    const result = spectator.service.canExclude(mockedEntry);

    expect(result).toBeFalsy();
  });

});
