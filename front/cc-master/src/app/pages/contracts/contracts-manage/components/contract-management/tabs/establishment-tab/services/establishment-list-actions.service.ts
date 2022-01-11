import { Injectable } from '@angular/core';
import { LuModal } from '@lucca-front/ng/modal';
import { Observable, ReplaySubject } from 'rxjs';
import { take } from 'rxjs/operators';

import {
  AttachmentDeletionModalComponent,
  AttachmentEndEditionModalComponent,
  AttachmentEndEditionModalMode,
  AttachmentExclusionModalComponent,
  AttachmentLinkingModalComponent,
  AttachmentStartEditionModalComponent,
  IAttachmentEndEditionModalData,
  IAttachmentExclusionModalData,
  IAttachmentLinkingModalData,
  IAttachmentStartEditionModalData,
} from '../components/modals';
import {
  IAttachmentDeletionModalData,
} from '../components/modals/deletion-modal/attachment-deletion-modal-data.interface';
import { IContractEstablishment } from '../models/contract-establishment.interface';
import { IEstablishmentAttachment } from '../models/establishment-attachment.interface';
import { IEstablishmentContract } from '../models/establishment-contract.interface';
import { IEstablishmentContractProduct } from '../models/establishment-contract-product.interface';

@Injectable()
export class EstablishmentListActionsService {

  public get refreshList$(): Observable<void> {
    return this.refreshListSubject.asObservable();
  }

  private refreshListSubject: ReplaySubject<void> = new ReplaySubject<void>(1);

  constructor(private luModal: LuModal) {
  }

  public openAttachmentDeletion(attachmentId: number): void {
    this.openAttachmentsDeletion([attachmentId]);
  }

  public openAttachmentsDeletion(attachmentIds: number[]): void {
    const modalData: IAttachmentDeletionModalData = { attachmentIds };
    const luModalRef = this.luModal.open(AttachmentDeletionModalComponent, modalData);
    luModalRef.onClose.pipe(take(1)).subscribe(() => this.updateList());
  }

  public openAttachmentFutureDeactivationEdition(
    establishment: IContractEstablishment,
    attachment: IEstablishmentAttachment,
    lastCountPeriod: Date,
    contractCloseOn: string,
  ): void {
    this.openAttachmentsFutureDeactivationEdition([establishment], [attachment], lastCountPeriod, contractCloseOn);
  }

  public openAttachmentsFutureDeactivationEdition(
    establishments: IContractEstablishment[],
    attachments: IEstablishmentAttachment[],
    lastCountPeriod: Date,
    contractCloseOn: string,
  ): void {
    const modalData: IAttachmentEndEditionModalData = {
      mode: AttachmentEndEditionModalMode.FutureDeactivationEdition,
      establishments,
      attachments,
      lastCountPeriod,
      contractCloseOn,
    };

    const luModalRef = this.luModal.open(AttachmentEndEditionModalComponent, modalData);
    luModalRef.onClose.pipe(take(1)).subscribe(() => this.updateList());
  }

  public openAttachmentFutureActivationEdition(
    establishment: IContractEstablishment,
    attachment: IEstablishmentAttachment,
    lastCountPeriod: Date,
    contractStartDate: Date,
  ): void {
    this.openAttachmentsFutureActivationEdition([establishment], [attachment], lastCountPeriod, contractStartDate);
  }

  public openAttachmentsFutureActivationEdition(
    establishments: IContractEstablishment[],
    attachments: IEstablishmentAttachment[],
    lastCountPeriod: Date,
    contractStartDate: Date,
  ): void {
    const modalData: IAttachmentStartEditionModalData = {
      establishments,
      attachments,
      lastCountPeriod,
      contractStartDate,
    };

    const luModalRef = this.luModal.open(AttachmentStartEditionModalComponent, modalData);
    luModalRef.onClose.pipe(take(1)).subscribe(() => this.updateList());
  }

  public openAttachmentLinking(
    establishment: IContractEstablishment,
    attachment: IEstablishmentAttachment,
    contract: IEstablishmentContract,
  ): void {
    this.openAttachmentsLinking([establishment], [attachment], contract);
  }

  public openAttachmentsLinking(
    establishments: IContractEstablishment[],
    attachments: IEstablishmentAttachment[],
    contract: IEstablishmentContract,
  ): void {
    const modalData: IAttachmentLinkingModalData = { establishments, attachments, contract };

    const luModalRef = this.luModal.open(AttachmentLinkingModalComponent, modalData);
    luModalRef.onClose.pipe(take(1)).subscribe(() => this.updateList());
  }

  public openAttachmentUnlinking(
    establishment: IContractEstablishment,
    attachment: IEstablishmentAttachment,
    lastCountPeriod: Date,
    contractCloseOn: string,
  ): void {
    this.openAttachmentsUnlinking([establishment], [attachment], lastCountPeriod, contractCloseOn);
  }

  public openAttachmentsUnlinking(
    establishments: IContractEstablishment[],
    attachments: IEstablishmentAttachment[],
    lastCountPeriod: Date,
    contractCloseOn: string,
  ): void {
    const modalData: IAttachmentEndEditionModalData = {
      mode: AttachmentEndEditionModalMode.Unlinking,
      establishments,
      attachments,
      lastCountPeriod,
      contractCloseOn,
      description: 'front_contractPage_establishments_unlink_modal_description',
    };

    const luModalRef = this.luModal.open(AttachmentEndEditionModalComponent, modalData);
    luModalRef.onClose.pipe(take(1)).subscribe(() => this.updateList());
  }

  public openAttachmentUnlinkingCancellation(
    establishment: IContractEstablishment,
    attachment: IEstablishmentAttachment,
    contractCloseOn: string,
  ): void {
    const modalData: IAttachmentEndEditionModalData = {
      mode: AttachmentEndEditionModalMode.UnlinkingCancellation,
      establishments: [establishment],
      attachments: [attachment],
      contractCloseOn,
      description: 'front_contractPage_establishments_cancelUnlinking_modal_description',
    };

    const luModalRef = this.luModal.open(AttachmentEndEditionModalComponent, modalData);
    luModalRef.onClose.pipe(take(1)).subscribe(() => this.updateList());
  }

  public openAttachmentExclusion(establishment: IContractEstablishment, product: IEstablishmentContractProduct): void {
    this.openAttachmentsExclusion([establishment], product);
  }

  public openAttachmentsExclusion(establishments: IContractEstablishment[], product: IEstablishmentContractProduct): void {
    const modalData: IAttachmentExclusionModalData = { establishments, product };
    const luModalRef = this.luModal.open(AttachmentExclusionModalComponent, modalData);
    luModalRef.onClose.pipe(take(1)).subscribe(() => this.updateList());
  }

  public redirectToContract(contractId: number): void {
    window.open(`contracts/manage/${ contractId }/legal-entities`);
  }

  private updateList(): void {
    this.refreshListSubject.next();
  }
}
