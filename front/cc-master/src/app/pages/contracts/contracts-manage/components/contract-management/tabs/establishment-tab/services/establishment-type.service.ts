import { Injectable } from '@angular/core';

import { IContractEstablishment } from '../models/contract-establishment.interface';
import { IEstablishmentAttachment } from '../models/establishment-attachment.interface';
import { IEstablishmentContract } from '../models/establishment-contract.interface';
import { IEstablishmentWithAttachments } from '../models/establishment-with-attachments.interface';
import { IEstablishmentsWithAttachmentsByType } from '../models/establishments-by-type.interface';

@Injectable()
export class EstablishmentTypeService {

  public getEstablishmentListEntriesByType(
    entries: IEstablishmentWithAttachments[],
    contract: IEstablishmentContract,
  ): IEstablishmentsWithAttachmentsByType {
    return ({
      excluded: this.getExcludedEts(entries, contract.id, contract.productId),
      withError: this.getEtsWithError(entries, contract.productId),
      linkedToContract: this.getEtsLinkedToContract(entries, contract.id, contract.productId),
      linkedToAnotherContract: this.getEtsLinkedToAnotherContract(entries, contract.id, contract.productId),
    });
  }

  private getExcludedEts(
    entries: IEstablishmentWithAttachments[],
    contractId: number,
    productId: number,
  ): IEstablishmentWithAttachments[] {
    return entries.filter(e => this.isExcluded(e.establishment, productId) && !this.isLinkedToContract(e, contractId, productId));
  }

  private getEtsWithError(entries: IEstablishmentWithAttachments[], productId: number): IEstablishmentWithAttachments[] {
    return entries.filter(e => this.isConsideredAsError(e, productId));
  }

  private getEtsLinkedToContract(
    entries: IEstablishmentWithAttachments[],
    contractId: number,
    productId: number,
  ): IEstablishmentWithAttachments[] {
    return entries.filter(e => this.isLinkedToContract(e, contractId, productId));
  }

  private getEtsLinkedToAnotherContract(
    entries: IEstablishmentWithAttachments[],
    contractId: number,
    productId: number,
  ): IEstablishmentWithAttachments[] {
    return entries.filter(e => this.isLinkedToAnotherContract(e, contractId, productId));
  }

  private isExcluded(establishment: IContractEstablishment, productId: number): boolean {
    return !!establishment.excludedEntities.find(e => e.productId === productId);
  }

  private isConsideredAsError(ets: IEstablishmentWithAttachments, productId: number): boolean {
    if (this.isExcluded(ets.establishment, productId)) {
      return false;
    }

    const hasAttachments = !!ets.currentAttachment || !!ets.nextAttachment;
    return !hasAttachments || !ets.establishment.isActive;
  }

  private isLinkedToContract(ets: IEstablishmentWithAttachments, contractId: number, productId: number): boolean {
    if (this.isConsideredAsError(ets, productId) || !this.isLinked(ets)) {
      return false;
    }

    return this.getReferencedCovering(ets).contractID === contractId;
  }

  private isLinkedToAnotherContract(ets: IEstablishmentWithAttachments, contractId: number, productId: number): boolean {
    if (this.isExcluded(ets.establishment, productId) || !this.isLinked(ets)) {
      return false;
    }

    return this.getReferencedCovering(ets).contractID !== contractId;
  }

  private isLinked(ets: IEstablishmentWithAttachments): boolean {
    return !!ets.currentAttachment || !!ets.nextAttachment;
  }

  private getReferencedCovering(ets: IEstablishmentWithAttachments): IEstablishmentAttachment {
    return !!ets.currentAttachment ? ets.currentAttachment : ets.nextAttachment;
  }
}
