import { createServiceFactory, SpectatorService } from '@ngneat/spectator';
import { addMonths, startOfMonth, subMonths } from 'date-fns';

import { AttachmentEndReason } from '../constants/attachment-end-reason.const';
import { IContractEstablishment } from '../models/contract-establishment.interface';
import { IEstablishmentAttachment } from '../models/establishment-attachment.interface';
import { IEstablishmentContract } from '../models/establishment-contract.interface';
import { IEstablishmentContractProduct } from '../models/establishment-contract-product.interface';
import { IEstablishmentExcludedEntity } from '../models/establishment-excluded-entity.interface';
import { IEstablishmentWithAttachments } from '../models/establishment-with-attachments.interface';
import { EstablishmentTypeService } from './establishment-type.service';

const fakeContract = (id: number, productId: number): IEstablishmentContract => ({
  id,
  name: 'fake-contract',
  product: { id: productId } as IEstablishmentContractProduct,
  productId,
  nbMonthTheorical: 0,
  environmentId: 1,
  theoricalStartOn: '2020-01-01',
});
const fakeEstablishment = (excludedEntities: IEstablishmentExcludedEntity[] = [], isActive: boolean = true): IContractEstablishment => ({
  id: 1,
  isActive,
  name: 'fake-establishment',
  excludedEntities,
  contractEntities: [],
});
const fakeAttachment = (start: Date, end: Date, contractID: number): IEstablishmentAttachment => ({
  start: start?.toString(),
  end: end?.toString(),
  id: 1,
  name: 'fake',
  nbMonthFree: 0,
  endReason: AttachmentEndReason.Modification,
  legalEntityId: 1,
  contractID,
  contract: {} as IEstablishmentContract,
});
const fakeEtsEntry = (
  establishment: IContractEstablishment,
  currentAttachment: IEstablishmentAttachment,
  nextAttachment: IEstablishmentAttachment,
): IEstablishmentWithAttachments => ({
  establishment,
  currentAttachment,
  nextAttachment,
  lastAttachment: null,
});

const productId = 1;
const contract = fakeContract(1, productId);

const today = new Date();
const nextMonth = addMonths(today, 1);
const lastMonth = subMonths(today, 1);

describe('EstablishmentTypeService', () => {
  let spectator: SpectatorService<EstablishmentTypeService>;
  const createService = createServiceFactory({
    service: EstablishmentTypeService,
  });

  beforeEach(() => spectator = createService());

  it('should get excluded establishments', () => {
    const excludedEntityWithSameProduct: IEstablishmentExcludedEntity = ({ id: 1, productId, legalEntityID: 1 });
    const excludedEstablishment = fakeEstablishment([excludedEntityWithSameProduct]);
    const excludedEntry = fakeEtsEntry(excludedEstablishment, null, null);

    const result = spectator.service.getEstablishmentListEntriesByType([excludedEntry], contract);

    expect(result.excluded).toEqual([excludedEntry]);
    expect(result.linkedToAnotherContract).toEqual([]);
    expect(result.linkedToContract).toEqual([]);
    expect(result.withError).toEqual([]);
  });

  it('should get excluded establishments with attachments linked to contract', () => {
    const excludedEntityWithSameProduct: IEstablishmentExcludedEntity = ({ id: 1, productId, legalEntityID: 1 });
    const excludedEstablishment = fakeEstablishment([excludedEntityWithSameProduct]);
    const excludedEntry = fakeEtsEntry(excludedEstablishment, null, null);
    const currentAttachment = fakeAttachment(startOfMonth(lastMonth), null, contract.id);
    const excludedEntryWithAttachmentLinkedToContract = fakeEtsEntry(excludedEstablishment, currentAttachment, null);
    const entries = [excludedEntry, excludedEntryWithAttachmentLinkedToContract];

    const result = spectator.service.getEstablishmentListEntriesByType(entries, contract);

    expect(result.excluded).toEqual([excludedEntry]);
    expect(result.linkedToAnotherContract).toEqual([]);
    expect(result.linkedToContract).toEqual([excludedEntryWithAttachmentLinkedToContract]);
    expect(result.withError).toEqual([]);
  });

  it('should get excluded establishments with attachments linked to another contract', () => {
    const excludedEntityWithSameProduct: IEstablishmentExcludedEntity = ({ id: 1, productId, legalEntityID: 1 });
    const excludedEstablishment = fakeEstablishment([excludedEntityWithSameProduct]);
    const excludedEntry = fakeEtsEntry(excludedEstablishment, null, null);
    const anotherContract = fakeContract(2, productId);
    const attachmentToAnotherContract = fakeAttachment(startOfMonth(lastMonth), null, anotherContract.id);
    const excludedEntryWithAttachmentLinkedToAnotherContract = fakeEtsEntry(excludedEstablishment, attachmentToAnotherContract, null);
    const entries = [excludedEntry, excludedEntryWithAttachmentLinkedToAnotherContract];

    const result = spectator.service.getEstablishmentListEntriesByType(entries, contract);

    expect(result.excluded).toEqual([excludedEntry, excludedEntryWithAttachmentLinkedToAnotherContract]);
    expect(result.linkedToAnotherContract).toEqual([]);
    expect(result.linkedToContract).toEqual([]);
    expect(result.withError).toEqual([]);
  });

  it('should get linked to another contract establishments', () => {
    const anotherContract = fakeContract(2, productId);
    const currentAttachmentToAnotherContract = fakeAttachment(startOfMonth(lastMonth), null, anotherContract.id);
    const nextAttachmentToAnotherContract = fakeAttachment(startOfMonth(nextMonth), null, anotherContract.id);
    const entriesLinkedToAnotherContract = [
      fakeEtsEntry(fakeEstablishment(), currentAttachmentToAnotherContract, null),
      fakeEtsEntry(fakeEstablishment(), null, nextAttachmentToAnotherContract),
    ];

    const result = spectator.service.getEstablishmentListEntriesByType(entriesLinkedToAnotherContract, contract);

    expect(result.excluded).toEqual([]);
    expect(result.linkedToAnotherContract).toEqual(entriesLinkedToAnotherContract);
    expect(result.linkedToContract).toEqual([]);
    expect(result.withError).toEqual([]);
  });

  it('should get linked to this contract establishments', () => {
    const currentAttachment = fakeAttachment(startOfMonth(lastMonth), null, contract.id);
    const nextAttachment = fakeAttachment(startOfMonth(nextMonth), null, contract.id);
    const entriesLinkedToContract = [
      fakeEtsEntry(fakeEstablishment(), currentAttachment, null),
      fakeEtsEntry(fakeEstablishment(), null, nextAttachment),
    ];

    const result = spectator.service.getEstablishmentListEntriesByType(entriesLinkedToContract, contract);

    expect(result.excluded).toEqual([]);
    expect(result.linkedToAnotherContract).toEqual([]);
    expect(result.linkedToContract).toEqual(entriesLinkedToContract);
    expect(result.withError).toEqual([]);
  });

  it('should get linked to this contract establishments with excluded entities', () => {
    const excludedEntityWithSameProduct: IEstablishmentExcludedEntity = ({ id: 1, productId, legalEntityID: 1 });
    const currentAttachment = fakeAttachment(startOfMonth(lastMonth), null, contract.id);
    const nextAttachment = fakeAttachment(startOfMonth(nextMonth), null, contract.id);
    const entriesLinkedToContract = [
      fakeEtsEntry(fakeEstablishment(), currentAttachment, null),
      fakeEtsEntry(fakeEstablishment([excludedEntityWithSameProduct]), null, nextAttachment),
    ];

    const result = spectator.service.getEstablishmentListEntriesByType(entriesLinkedToContract, contract);

    expect(result.excluded).toEqual([]);
    expect(result.linkedToAnotherContract).toEqual([]);
    expect(result.linkedToContract).toEqual(entriesLinkedToContract);
    expect(result.withError).toEqual([]);
  });

  it('should get error establishments because it does not have attachments', () => {
    const errorEntries = [fakeEtsEntry(fakeEstablishment(), null, null)];

    const result = spectator.service.getEstablishmentListEntriesByType(errorEntries, contract);

    expect(result.excluded).toEqual([]);
    expect(result.linkedToAnotherContract).toEqual([]);
    expect(result.linkedToContract).toEqual([]);
    expect(result.withError).toEqual(errorEntries);
  });

  it('should get error establishments because it is inactive', () => {
    const currentAttachment = fakeAttachment(startOfMonth(lastMonth), null, contract.id);
    const inactiveEstablishment = fakeEstablishment([], false);
    const errorEntries = [fakeEtsEntry(inactiveEstablishment, currentAttachment, null)];

    const result = spectator.service.getEstablishmentListEntriesByType(errorEntries, contract);

    expect(result.excluded).toEqual([]);
    expect(result.linkedToAnotherContract).toEqual([]);
    expect(result.linkedToContract).toEqual([]);
    expect(result.withError).toEqual(errorEntries);
  });
});
