import { Injectable } from '@angular/core';
import { isFuture } from 'date-fns';

import { IContractEstablishment } from '../models/contract-establishment.interface';
import { IEstablishmentAttachment } from '../models/establishment-attachment.interface';
import { IEstablishmentContract } from '../models/establishment-contract.interface';
import { IEstablishmentContractProduct } from '../models/establishment-contract-product.interface';
import { IEstablishmentWithAttachments } from '../models/establishment-with-attachments.interface';
import { IEstablishmentsWithAttachmentsByType } from '../models/establishments-by-type.interface';

@Injectable()
export class EstablishmentTypeService {

  public getEstablishmentsByType(
    entries: IEstablishmentWithAttachments[],
    contract: IEstablishmentContract,
  ): IEstablishmentsWithAttachmentsByType {
    const contractSolutionIds = contract?.product?.solutions.map(s => s.id) ?? [];
    return ({
      excluded: this.getExcludedEts(entries, contract.id, contractSolutionIds),
      withError: entries.filter(e => this.isConsideredAsError(e.establishment, contractSolutionIds)),
      linkedToContract: this.getEtsLinkedToContract(entries, contract.id),
      linkedToAnotherContract: this.getEtsLinkedToAnotherContract(entries, contract.id, contractSolutionIds),
    });
  }

  private getExcludedEts(
    entries: IEstablishmentWithAttachments[],
    contractId: number,
    contractSolutionIds: number[],
  ): IEstablishmentWithAttachments[] {
    return entries.filter(e => this.isExcluded(e.establishment, contractSolutionIds) && !this.isLinkedToContract(e, contractId));
  }

  private getEtsLinkedToContract(entries: IEstablishmentWithAttachments[], contractId: number): IEstablishmentWithAttachments[] {
    return entries.filter(e => this.isLinkedToContract(e, contractId));
  }

  private getEtsLinkedToAnotherContract(
    entries: IEstablishmentWithAttachments[],
    contractId: number,
    contractSolutionIds: number[],
  ): IEstablishmentWithAttachments[] {
    return entries.filter(e => this.isLinkedToAnotherContract(e, contractId) && !this.isExcluded(e.establishment, contractSolutionIds));
  }

  private isExcluded(establishment: IContractEstablishment, contractSolutionIds: number[]): boolean {
    return !!establishment.excludedEntities.find(e => contractSolutionIds.includes(e.solutionId));
  }

  private isLinkedToContract(ets: IEstablishmentWithAttachments, contractId: number): boolean {
    return this.isLinked(ets) && this.getReferencedCovering(ets).contractID === contractId;
  }

  private isLinkedToAnotherContract(ets: IEstablishmentWithAttachments, contractId: number): boolean {
    return this.isLinked(ets) && this.getReferencedCovering(ets).contractID !== contractId;
  }

  private isLinked(ets: IEstablishmentWithAttachments): boolean {
    const hasAttachments = !!ets.currentAttachment || !!ets.nextAttachment;
    return hasAttachments && ets.establishment.isActive;
  }

  private getReferencedCovering(ets: IEstablishmentWithAttachments): IEstablishmentAttachment {
    return !!ets.currentAttachment ? ets.currentAttachment : ets.nextAttachment;
  }

  private isConsideredAsError(establishment: IContractEstablishment, solutionIds: number[]): boolean {
    return !this.hasNotFinishedAttachments(establishment, solutionIds)
      && !this.isExcluded(establishment, solutionIds);
  }

  private hasNotFinishedAttachments(establishment: IContractEstablishment, solutionIds: number[]): boolean {
    const attachmentsForThisSolutions = establishment.contractEntities
      .filter(a => !this.hasMultiSolutions(a.contract.product) || this.hasSomeSolutions(a.contract.product, solutionIds));

    const notFinishedAttachments = attachmentsForThisSolutions.filter(a => !!a?.start && (!a?.end || isFuture(new Date(a.end))));
    return !!notFinishedAttachments.length;
  }

  private hasMultiSolutions(product: IEstablishmentContractProduct): boolean {
    return !!product?.solutions?.length;
  }

  private hasSomeSolutions(product: IEstablishmentContractProduct, solutionIds: number[]): boolean {
    return product?.solutions?.some(solution => solutionIds.includes(solution.id));
  }
}
