import { ISolution } from '@cc/domain/billing/offers';
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

const fakeContract = (id: number, product: IEstablishmentContractProduct): IEstablishmentContract => ({
  id,
  name: 'fake-contract',
  product,
  productId: product.id,
  nbMonthTheorical: 0,
  environmentId: 1,
  theoricalStartOn: '2020-01-01',
});
const fakeEstablishment = (
  excludedEntities: IEstablishmentExcludedEntity[] = [],
  attachments: IEstablishmentAttachment[] = [],
  isActive: boolean = true,
): IContractEstablishment => ({
  id: 1,
  isActive,
  name: 'fake-establishment',
  excludedEntities,
  contractEntities: attachments,
});
const fakeAttachment = (start: Date, end: Date, contractID: number, contract?: IEstablishmentContract): IEstablishmentAttachment => ({
  start: start?.toString(),
  end: end?.toString(),
  id: 1,
  name: 'fake',
  nbMonthFree: 0,
  endReason: AttachmentEndReason.Modification,
  legalEntityId: 1,
  contractID,
  contract,
});
const fakeExcludedEts = (solutionId: number, establishmentId: number): IEstablishmentExcludedEntity => ({
  id: 1,
  solutionId,
  solution: { id: solutionId, name: 'solution-name' },
  legalEntityID: establishmentId,
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
const fakeProduct = (id: number, solutions: ISolution[]) => ({
  id,
  name: 'product-with-one-solution',
  isMultiSuite: solutions.length > 1,
  solutions,
});

const productWithOneSolution = fakeProduct(1, [{ id: 1, name: 'solution-1' }]);
const contract = fakeContract(1, productWithOneSolution);

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
    const solutionId = contract.product.solutions[0].id;
    const excludedEntityWithSameContractSolution = fakeExcludedEts(solutionId, 1);
    const excludedEstablishment = fakeEstablishment([excludedEntityWithSameContractSolution]);
    const excludedEntry = fakeEtsEntry(excludedEstablishment, null, null);

    const result = spectator.service.getEstablishmentsByType([excludedEntry], contract);

    expect(result.excluded).toEqual([excludedEntry]);
    expect(result.linkedToAnotherContract).toEqual([]);
    expect(result.linkedToContract).toEqual([]);
    expect(result.withError).toEqual([]);
  });

  it('should get excluded establishments with attachments linked to contract', () => {
    const solutionId = contract.product.solutions[0].id;
    const excludedEntityWithSameContractSolution = fakeExcludedEts(solutionId, 1);
    const excludedEstablishment = fakeEstablishment([excludedEntityWithSameContractSolution]);
    const excludedEntry = fakeEtsEntry(excludedEstablishment, null, null);
    const currentAttachment = fakeAttachment(startOfMonth(lastMonth), null, contract.id);
    const excludedEntryWithAttachmentLinkedToContract = fakeEtsEntry(excludedEstablishment, currentAttachment, null);
    const entries = [excludedEntry, excludedEntryWithAttachmentLinkedToContract];

    const result = spectator.service.getEstablishmentsByType(entries, contract);

    expect(result.excluded).toEqual([excludedEntry]);
    expect(result.linkedToAnotherContract).toEqual([]);
    expect(result.linkedToContract).toEqual([excludedEntryWithAttachmentLinkedToContract]);
    expect(result.withError).toEqual([]);
  });

  it('should get excluded establishments with attachments linked to another contract', () => {
    const solutionId = contract.product.solutions[0].id;
    const excludedEntityWithSameContractSolution = fakeExcludedEts(solutionId, 1);
    const excludedEstablishment = fakeEstablishment([excludedEntityWithSameContractSolution]);
    const excludedEntry = fakeEtsEntry(excludedEstablishment, null, null);
    const anotherContract = fakeContract(2, productWithOneSolution);
    const attachmentToAnotherContract = fakeAttachment(startOfMonth(lastMonth), null, anotherContract.id);
    const excludedEntryWithAttachmentLinkedToAnotherContract = fakeEtsEntry(excludedEstablishment, attachmentToAnotherContract, null);
    const entries = [excludedEntry, excludedEntryWithAttachmentLinkedToAnotherContract];

    const result = spectator.service.getEstablishmentsByType(entries, contract);

    expect(result.excluded).toEqual([excludedEntry, excludedEntryWithAttachmentLinkedToAnotherContract]);
    expect(result.linkedToAnotherContract).toEqual([]);
    expect(result.linkedToContract).toEqual([]);
    expect(result.withError).toEqual([]);
  });

  it('should get linked to another contract establishments', () => {
    const anotherContract = fakeContract(2, productWithOneSolution);
    const currentAttachment = fakeAttachment(startOfMonth(lastMonth), null, anotherContract.id, anotherContract);
    const nextAttachment = fakeAttachment(startOfMonth(nextMonth), null, anotherContract.id, anotherContract);
    const etsWithCurrentAttachment = fakeEstablishment([], [currentAttachment]);
    const etsWithNextAttachment = fakeEstablishment([], [nextAttachment]);

    const entriesLinkedToAnotherContract = [
      fakeEtsEntry(etsWithCurrentAttachment, currentAttachment, null),
      fakeEtsEntry(etsWithNextAttachment, null, nextAttachment),
    ];

    const result = spectator.service.getEstablishmentsByType(entriesLinkedToAnotherContract, contract);

    expect(result.excluded).toEqual([]);
    expect(result.linkedToAnotherContract).toEqual(entriesLinkedToAnotherContract);
    expect(result.linkedToContract).toEqual([]);
    expect(result.withError).toEqual([]);
  });

  it('should get linked to this contract establishments', () => {
    const currentAttachment = fakeAttachment(startOfMonth(lastMonth), null, contract.id, contract);
    const nextAttachment = fakeAttachment(startOfMonth(nextMonth), null, contract.id, contract);
    const entriesLinkedToContract = [
      fakeEtsEntry(fakeEstablishment([], [currentAttachment]), currentAttachment, null),
      fakeEtsEntry(fakeEstablishment([], [nextAttachment]), null, nextAttachment),
    ];

    const result = spectator.service.getEstablishmentsByType(entriesLinkedToContract, contract);

    expect(result.excluded).toEqual([]);
    expect(result.linkedToAnotherContract).toEqual([]);
    expect(result.linkedToContract).toEqual(entriesLinkedToContract);
    expect(result.withError).toEqual([]);
  });

  it('should get linked to this contract establishments with excluded entities', () => {
    const anotherSolutionId = 2;
    const excludedEntityWithSameContractSolution = fakeExcludedEts(anotherSolutionId, 1);
    const currentAttachment = fakeAttachment(startOfMonth(lastMonth), null, contract.id, contract);
    const nextAttachment = fakeAttachment(startOfMonth(nextMonth), null, contract.id, contract);
    const entriesLinkedToContract = [
      fakeEtsEntry(fakeEstablishment([], [currentAttachment]), currentAttachment, null),
      fakeEtsEntry(fakeEstablishment([excludedEntityWithSameContractSolution], [currentAttachment]), null, nextAttachment),
    ];

    const result = spectator.service.getEstablishmentsByType(entriesLinkedToContract, contract);

    expect(result.excluded).toEqual([]);
    expect(result.linkedToAnotherContract).toEqual([]);
    expect(result.linkedToContract).toEqual(entriesLinkedToContract);
    expect(result.withError).toEqual([]);
  });

  it('should get error establishments because it does not have attachments', () => {
    const errorEntries = [fakeEtsEntry(fakeEstablishment(), null, null)];

    const result = spectator.service.getEstablishmentsByType(errorEntries, contract);

    expect(result.excluded).toEqual([]);
    expect(result.linkedToAnotherContract).toEqual([]);
    expect(result.linkedToContract).toEqual([]);
    expect(result.withError).toEqual(errorEntries);
  });

  it('should get error establishments because it is inactive', () => {
    const currentAttachment = fakeAttachment(startOfMonth(lastMonth), null, contract.id, contract);
    const inactiveEstablishment = fakeEstablishment([], [], false);
    const errorEntries = [fakeEtsEntry(inactiveEstablishment, currentAttachment, null)];

    const result = spectator.service.getEstablishmentsByType(errorEntries, contract);

    expect(result.excluded).toEqual([]);
    expect(result.linkedToAnotherContract).toEqual([]);
    expect(result.linkedToContract).toEqual([]);
    expect(result.withError).toEqual(errorEntries);
  });

  it('should get error establishments because it has attachment but not for this solution', () => {
    const product = fakeProduct(22, [{ id: 4, name: 'solution-4' }, { id: 5, name: 'solution-5' }]);
    const anotherContract = fakeContract(2, product);
    const attachment = fakeAttachment(startOfMonth(lastMonth), null, anotherContract.id, anotherContract);
    const establishment = fakeEstablishment([], [attachment]);

    const entries = [fakeEtsEntry(establishment, null, null)];

    const result = spectator.service.getEstablishmentsByType(entries, contract);

    expect(result.excluded).toEqual([]);
    expect(result.linkedToAnotherContract).toEqual([]);
    expect(result.linkedToContract).toEqual([]);
    expect(result.withError).toEqual(entries);
  });
});
