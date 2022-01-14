import { Injectable } from '@angular/core';
import { ISolution } from '@cc/domain/billing/offers';
import { from, Observable, of, pipe, UnaryFunction } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { IContractEstablishment } from '../models/contract-establishment.interface';
import { IEstablishmentAttachment } from '../models/establishment-attachment.interface';
import { IEstablishmentContract } from '../models/establishment-contract.interface';
import { IEstablishmentWithAttachments } from '../models/establishment-with-attachments.interface';
import { AttachmentsTimelineService } from './attachments-timeline.service';
import { EstablishmentProductStoreService } from './establishment-product-store.service';
import { ISolutionProduct } from './establishment-product-store-data.service';
import { EstablishmentsDataService } from './establishments-data.service';

@Injectable()
export class EstablishmentsWithAttachmentsService {
  constructor(
    private dataService: EstablishmentsDataService,
    private timelineService: AttachmentsTimelineService,
    private productsStoreService: EstablishmentProductStoreService,
  ) {
  }

  public getEstablishments$(contract: IEstablishmentContract): Observable<IEstablishmentWithAttachments[]> {
    if (!contract.environmentId) {
      return of([]);
    }

    const allEstablishments$ = this.dataService.getEstablishments$(contract.environmentId);

    const etsWithFilteredAttachments$ = allEstablishments$.pipe(
      switchMap(ets => from(this.getEtsWithFilteredAttachmentsAsync(ets, contract.product.solutions))));

    return etsWithFilteredAttachments$.pipe(this.toTimelineAttachmentsEts);
  }

  private async getEtsWithFilteredAttachmentsAsync(
    establishments: IContractEstablishment[],
    solutions: ISolution[],
  ): Promise<IContractEstablishment[]> {
    if (!establishments?.length) {
      return [];
    }

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

  private get toTimelineAttachmentsEts(): UnaryFunction<Observable<IContractEstablishment[]>, Observable<IEstablishmentWithAttachments[]>> {
    return pipe(
      map(establishments => establishments.map(e => this.toTimelineAttachment(e))),
      map(establishments => this.sort(establishments)),
    );
  }

  private toTimelineAttachment(establishment: IContractEstablishment): IEstablishmentWithAttachments {
    return {
      establishment,
      currentAttachment: this.timelineService.getCurrentAttachment(establishment.contractEntities),
      nextAttachment: this.timelineService.getNextAttachment(establishment.contractEntities),
      lastAttachment: this.timelineService.getLastAttachment(establishment.contractEntities),
    };
  }

  private sort(establishments: IEstablishmentWithAttachments[]): IEstablishmentWithAttachments[] {
    return establishments.sort((a, b) => this.getStartDate(a).getTime() - this.getStartDate(b).getTime());
  }

  private getStartDate(establishmentWithAttachments: IEstablishmentWithAttachments): Date {
    const attachment = establishmentWithAttachments.currentAttachment || establishmentWithAttachments.nextAttachment;
    return new Date(attachment?.start);
  }
}
