import { Injectable } from '@angular/core';
import { ISolution } from '@cc/domain/billing/offers';
import { from, Observable } from 'rxjs';

import { IContractEstablishment } from '../models/contract-establishment.interface';
import { IEstablishmentAttachment } from '../models/establishment-attachment.interface';
import { IEstablishmentContract } from '../models/establishment-contract.interface';
import { IListEntry, LifecycleStep, ListEntryType } from '../models/establishment-list-entry.interface';
import { AttachmentsTimelineService } from './attachments-timeline.service';
import { EstablishmentProductStoreService } from './establishment-product-store.service';
import { ISolutionProduct } from './establishment-product-store-data.service';
import { EstablishmentsDataService } from './establishments-data.service';
import { EstablishmentsTimelineService } from './establishments-timeline.service';

@Injectable()
export class EstablishmentListEntriesService {

  constructor(
    private dataService: EstablishmentsDataService,
    private establishmentsTimelineService: EstablishmentsTimelineService,
    private attachmentsTimelineService: AttachmentsTimelineService,
    private productsStoreService: EstablishmentProductStoreService,
  ) {
  }

  public getListEntries$(contract: IEstablishmentContract): Observable<IListEntry[]> {
    return from(this.getListEntriesAsync(contract));
  }

  private async getListEntriesAsync(contract: IEstablishmentContract): Promise<IListEntry[]> {
    if (!contract.environmentId) {
      return Promise.resolve([]);
    }

    const allEstablishments = await this.dataService.getEstablishments$(contract.environmentId).toPromise();
    const establishments = await this.getEtsWithFilteredAttachmentsAsync(allEstablishments, contract.product.solutions);

    const linkedToThisContract = this.getLinkedToThisContractEntries(establishments, contract.id);
    const linkedToAnotherContract =this.getLinkedToAnotherContractEntries(establishments, contract.id);
    const excluded = this.getExcludedEntries(establishments, contract);

    const excludedEstablishmentIds = excluded.map(e => e.establishment.id);
    const notExcludedEstablishments = establishments.filter(e => !excludedEstablishmentIds.includes(e.id));
    const withError = this.getErrorEntries(notExcludedEstablishments);

    return [...linkedToThisContract, ...linkedToAnotherContract, ...excluded, ...withError];
  }

  private async getEtsWithFilteredAttachmentsAsync(
    establishments: IContractEstablishment[],
    solutions: ISolution[],
  ): Promise<IContractEstablishment[]> {
    const productIds = this.getProductIdsByEts(establishments);
    const allProducts = await this.productsStoreService.getProducts$(productIds).toPromise();

    for (const establishment of establishments) {
      const attachments = establishment.contractEntities;
      establishment.contractEntities = this.getFilteredAttachments(attachments, solutions, allProducts);
    }

    return establishments;
  }

  private getFilteredAttachments(
    attachments: IEstablishmentAttachment[],
    solutions: ISolution[],
    allProducts: ISolutionProduct[],
  ): IEstablishmentAttachment[] {
    const filteredAttachments: IEstablishmentAttachment[] = [];

    for (const attachment of attachments) {
      const attachmentSolutions = allProducts.find(p => p.id === attachment.contract.productId)?.solutions ?? [];
      const attachmentSolutionIds = attachmentSolutions.map(s => s.id);

      if (solutions.some(solution => attachmentSolutionIds.includes(solution.id))) {
        filteredAttachments.push(attachment);
      }
    }

    return filteredAttachments;
  }

  private getProductIdsByEts(establishments: IContractEstablishment[]): number[] {
    return establishments
      .map(establishment => establishment.contractEntities.map(a => a.contract.productId))
      .reduce((flattened, productIds) => [...flattened, ...productIds])
      .filter((value, index, self) => self.indexOf(value) === index);
  }

  private getLinkedToThisContractEntries(establishments: IContractEstablishment[], contractId: number): IListEntry[] {
    const type = ListEntryType.LinkedToThisContract;

    const entries: IListEntry[] = [];
    for (const establishment of establishments) {
      for (const attachment of establishment.contractEntities) {
        if (!!attachment.contractID && attachment.contractID !== contractId) {
          continue;
        }

        const lifecycleStep = this.attachmentsTimelineService.getLifecycleStep(attachment);
        entries.push(({ establishment, attachment, lifecycleStep, type }));
      }
    }
    return entries;
  }

  private getLinkedToAnotherContractEntries(establishments: IContractEstablishment[], contractId: number): IListEntry[] {
    const type = ListEntryType.LinkedToAnotherContract;

    const entries: IListEntry[] = [];
    for (const establishment of establishments) {
      for (const attachment of establishment.contractEntities) {
        if (!!attachment.contractID && attachment.contractID === contractId) {
          continue;
        }

        const lifecycleStep = this.attachmentsTimelineService.getLifecycleStep(attachment);
        entries.push(({ establishment, attachment, lifecycleStep, type }));
      }
    }
    return entries;
  }

  private getErrorEntries(establishments: IContractEstablishment[]): IListEntry[] {
    const type = ListEntryType.Error;

    const entries: IListEntry[] = [];
    for (const establishment of establishments) {
      const lifecycleStep = this.establishmentsTimelineService.getLifecycleStepError(establishment);

      if (!!lifecycleStep) {
        entries.push({ establishment, lifecycleStep, type });
      }
    }
    return entries;
  }

  private getExcludedEntries(
    establishments: IContractEstablishment[],
    contract: IEstablishmentContract,
  ): IListEntry[] {
    const type = ListEntryType.Excluded;

    const lifecycleStep = LifecycleStep.Excluded;
    const excludedEstablishments = establishments.filter(e => this.isExcluded(e, contract.productId));
    return excludedEstablishments.map(establishment => ({ establishment, lifecycleStep, type }));
  }

  private isExcluded(establishment: IContractEstablishment, productId: number): boolean {
    return establishment.excludedEntities.some(e => e.productId === productId);
  }
}
