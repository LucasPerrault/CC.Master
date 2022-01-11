import { addMonths, endOfMonth, startOfMonth, subMonths, subYears } from 'date-fns';

import { IContractEstablishment } from '../models/contract-establishment.interface';
import { IEstablishmentAttachment } from '../models/establishment-attachment.interface';
import {
  AttachmentEndEditionConditions,
  AttachmentLinkingConditions,
  AttachmentStartEditionConditions,
} from './attachments-action-conditions';

const fakeEstablishment = (id: number, contractEntities: IEstablishmentAttachment[]): IContractEstablishment => ({
  id,
  name: 'fake-establishment',
  contractEntities,
} as IContractEstablishment);

const fakeAttachment = (start: Date, end: Date, id: number = 1, contractID: number = 1): IEstablishmentAttachment => ({
  start: start?.toString(),
  end: end?.toString(),
  id,
  name: 'fake',
} as IEstablishmentAttachment);

describe('AttachmentLinkingConditions', () => {

  it('min date should be null', () => {
    const contractTheoreticalStartOn = null;
    const establishments = null;

    const result = AttachmentLinkingConditions.minDate(establishments, contractTheoreticalStartOn);

    expect(result).toEqual(null);
  });

  it('min date should be the first day of contract start month', () => {
    const contractTheoreticalStartOn = startOfMonth(Date.now());
    const establishments = null;

    const result = AttachmentLinkingConditions.minDate(establishments, contractTheoreticalStartOn.toString());

    expect(result).toEqual(contractTheoreticalStartOn);
  });

  it('min date should be the first day of next month of last attachment end month', () => {
    const contractTheoreticalStartOn = startOfMonth(Date.now());

    const start = subYears(startOfMonth(Date.now()), 2);
    const end = subYears(startOfMonth(Date.now()), 1);
    const lastEndedAttachment = fakeAttachment(start, addMonths(end, 4));
    const establishments = [
      fakeEstablishment(1, [fakeAttachment(start, end), lastEndedAttachment]),
      fakeEstablishment(1, [fakeAttachment(start, addMonths(end, 3))]),
    ];

    const expected = startOfMonth(addMonths(new Date(lastEndedAttachment.end), 1));

    const result = AttachmentLinkingConditions.minDate(establishments, contractTheoreticalStartOn.toString());

    expect(result).toEqual(expected);
  });

  it('max date should be null', () => {
    const closedDate = null;

    const result = AttachmentLinkingConditions.maxDate(closedDate);

    expect(result).toEqual(closedDate);
  });

  it('max date should be the first day of contract closed month', () => {
    const closedDate = endOfMonth(Date.now());

    const expected = startOfMonth(closedDate);

    const result = AttachmentLinkingConditions.maxDate(closedDate.toString());

    expect(result).toEqual(expected);
  });
});

describe('AttachmentStartDateEditionConditions', () => {

  it('min date should be null', () => {
    const lastCountPeriod = null;
    const attachments = null;

    const result = AttachmentStartEditionConditions.minDate(lastCountPeriod, attachments);

    expect(result).toEqual(null);
  });

  it('min date should be the first day of next month of last count period', () => {
    const lastCountPeriod = endOfMonth(Date.now());
    const attachments = null;

    const expected = startOfMonth(addMonths(lastCountPeriod, 1));

    const result = AttachmentStartEditionConditions.minDate(lastCountPeriod, attachments);

    expect(result).toEqual(expected);
  });

  it('min date should be the first day of last attachment start', () => {
    const lastCountPeriod = null;
    const start = subYears(startOfMonth(Date.now()), 2);
    const lastStartedAttachment = fakeAttachment(addMonths(start, 3), null);
    const attachments = [fakeAttachment(start, null), lastStartedAttachment, fakeAttachment(addMonths(start, 1), null)];

    const expected = startOfMonth(new Date(lastStartedAttachment.start));

    const result = AttachmentStartEditionConditions.minDate(lastCountPeriod, attachments);

    expect(result).toEqual(expected);
  });

  it('min date should be the max between last count period and last attachment start date', () => {
    const start = startOfMonth(Date.now());
    const lastCountPeriod = addMonths(start, 3);
    const lastStartedAttachment = fakeAttachment(addMonths(start, 3), null);
    const attachments = [fakeAttachment(start, null), lastStartedAttachment, fakeAttachment(addMonths(start, 1), null)];

    const expected = new Date(lastStartedAttachment.start);

    const result = AttachmentStartEditionConditions.minDate(lastCountPeriod, attachments);

    expect(result).toEqual(expected);
  });
});

describe('AttachmentEndDateEditionConditions', () => {

  it('min date should be null', () => {
    const lastCountPeriod = null;
    const attachments = null;

    const result = AttachmentEndEditionConditions.minDate(lastCountPeriod, attachments);

    expect(result).toEqual(null);
  });

  it('min date should be the first day of last count period', () => {
    const lastCountPeriod = endOfMonth(Date.now());
    const attachments = null;

    const expected = startOfMonth(lastCountPeriod);

    const result = AttachmentEndEditionConditions.minDate(lastCountPeriod, attachments);

    expect(result).toEqual(expected);
  });

  it('min date should be the first day of last attachment start', () => {
    const lastCountPeriod = null;
    const start = subYears(startOfMonth(Date.now()), 2);
    const lastStartedAttachment = fakeAttachment(addMonths(start, 3), null);
    const attachments = [fakeAttachment(start, null), lastStartedAttachment, fakeAttachment(addMonths(start, 1), null)];

    const expected = startOfMonth(new Date(lastStartedAttachment.start));

    const result = AttachmentEndEditionConditions.minDate(lastCountPeriod, attachments);

    expect(result).toEqual(expected);
  });

  it('min date should be the max between last count period and last attachment start date', () => {
    const start = startOfMonth(Date.now());
    const lastCountPeriod = addMonths(start, 4);
    const lastStartedAttachment = fakeAttachment(addMonths(start, 3), null);
    const attachments = [fakeAttachment(start, null), lastStartedAttachment, fakeAttachment(addMonths(start, 1), null)];

    const expected = lastCountPeriod;

    const result = AttachmentEndEditionConditions.minDate(lastCountPeriod, attachments);

    expect(result).toEqual(expected);
  });

  it('max date should be null', () => {
    const closeOn = null;
    const establishments = null;
    const attachmentsToEdit = null;

    const result = AttachmentEndEditionConditions.maxDate(closeOn, establishments, attachmentsToEdit);

    expect(result).toEqual(null);
  });

  it('max date should be the first day of contract closed month', () => {
    const closeOn = endOfMonth(Date.now());
    const start = subMonths(startOfMonth(closeOn), 5);
    const attachmentsToEdit = [fakeAttachment(start, null)];
    const establishments = [fakeEstablishment(1, attachmentsToEdit)];

    const expected = startOfMonth(closeOn);

    const resultWithoutEts = AttachmentEndEditionConditions.maxDate(closeOn.toString(), null, null);
    const resultWithEts = AttachmentEndEditionConditions.maxDate(closeOn.toString(), establishments, attachmentsToEdit);

    expect(resultWithoutEts).toEqual(expected);
    expect(resultWithEts).toEqual(expected);
  });

  it('max date should be the previous month of first attachment start', () => {
    const closeOn = endOfMonth(Date.now());
    const start = addMonths(startOfMonth(closeOn), 2);
    const firstAttachment = fakeAttachment(start, null);
    const establishments = [fakeEstablishment(1, [firstAttachment, fakeAttachment(addMonths(start, 4), null)])];

    const expected = startOfMonth(subMonths(start, 1));

    const result = AttachmentEndEditionConditions.maxDate(closeOn.toString(), establishments, []);

    expect(result).toEqual(expected);
  });
});
