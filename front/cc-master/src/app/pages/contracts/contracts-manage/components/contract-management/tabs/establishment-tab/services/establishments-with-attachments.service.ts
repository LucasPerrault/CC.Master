import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';

import { IContractEstablishment } from '../models/contract-establishment.interface';
import { IEstablishmentAttachment } from '../models/establishment-attachment.interface';
import { IEstablishmentContract } from '../models/establishment-contract.interface';
import { IEstablishmentContractProduct } from '../models/establishment-contract-product.interface';
import { IEstablishmentWithAttachments } from '../models/establishment-with-attachments.interface';
import { AttachmentsTimelineService } from './attachments-timeline.service';
import { EstablishmentsDataService } from './establishments-data.service';

@Injectable()
export class EstablishmentsWithAttachmentsService {
  constructor(
    private dataService: EstablishmentsDataService,
    private timelineService: AttachmentsTimelineService,
  ) {
  }

  public getEstablishments$(contract: IEstablishmentContract): Observable<IEstablishmentWithAttachments[]> {
    if (!contract.environmentId) {
      return of([]);
    }

    return this.dataService.getEstablishments$(contract.environmentId).pipe(
      map((ets: IContractEstablishment[]) => this.getEstablishmentsByProduct(contract.product, ets)),
      map((ets: IContractEstablishment[]) => this.getEstablishmentsWithAttachments(ets)),
      map((ets: IEstablishmentWithAttachments[]) => this.getSortedgetEstablishmentsWithAttachments(ets)),
    );
  }

  private getEstablishmentsByProduct(product: IEstablishmentContractProduct, ets: IContractEstablishment[]): IContractEstablishment[] {
    return ets.map(establishment => {
      establishment.contractEntities = this.getAttachmentsByProduct(product, establishment);
      return establishment;
    });
  }

  private getAttachmentsByProduct(
    product: IEstablishmentContractProduct,
    establishment: IContractEstablishment,
  ): IEstablishmentAttachment[] {
    return establishment.contractEntities.filter(attachment =>
      attachment.contract?.productId === product.id
      || product.isMultiSuite && this.hasChildProductEquals(product.id, attachment),
    );
  }

  private hasChildProductEquals(productId: number, attachment: IEstablishmentAttachment): boolean {
    const childProductIds = attachment.contract?.product.solutions.map(s => s.id);
    return childProductIds.includes(productId);
  }

  private getEstablishmentsWithAttachments(ets: IContractEstablishment[]): IEstablishmentWithAttachments[] {
    return ets.map(establishment => this.getEstablishmentWithAttachments(establishment));
  }

  private getEstablishmentWithAttachments(establishment: IContractEstablishment): IEstablishmentWithAttachments {
    return {
      establishment,
      currentAttachment: this.timelineService.getCurrentAttachment(establishment.contractEntities),
      nextAttachment: this.timelineService.getNextAttachment(establishment.contractEntities),
      lastAttachment: this.timelineService.getLastAttachment(establishment.contractEntities),
    };
  }

  private getSortedgetEstablishmentsWithAttachments(ets: IEstablishmentWithAttachments[]): IEstablishmentWithAttachments[] {
    return ets.sort((a, b) =>
      this.getAttachmentStartDate(a).getTime() - this.getAttachmentStartDate(b).getTime(),
    );
  }

  private getAttachmentStartDate(establishmentWithAttachments: IEstablishmentWithAttachments): Date {
    const attachment = establishmentWithAttachments.currentAttachment || establishmentWithAttachments.nextAttachment;
    return new Date(attachment?.start);
  }
}
