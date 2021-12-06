import { Component, Input } from '@angular/core';

import { EstablishmentType } from '../../constants/establishment-type.enum';
import { IContractEstablishment } from '../../models/contract-establishment.interface';
import { IEstablishmentActionsContext } from '../../models/establishment-actions-context.interface';
import { IEstablishmentAttachment } from '../../models/establishment-attachment.interface';
import { IEstablishmentWithAttachments } from '../../models/establishment-with-attachments.interface';
import { AttachmentsActionRestrictionsService } from '../../services/attachments-action-restrictions.service';
import { EstablishmentListActionsService } from '../../services/establishment-list-actions.service';

@Component({
  selector: 'cc-establishment-list-actions-single',
  templateUrl: './establishment-list-actions-single.component.html',
})
export class EstablishmentListActionsSingleComponent {
  @Input() public entry: IEstablishmentWithAttachments;
  @Input() public context: IEstablishmentActionsContext;

  public etsType = EstablishmentType;

  public get establishment(): IContractEstablishment {
    return this.entry.establishment;
  }

  public get currentOrFutureAttachment(): IEstablishmentAttachment {
    return this.entry.currentAttachment || this.entry.nextAttachment;
  }

  public get canDelete(): boolean {
    return this.actionRestrictionsService.canDelete(this.currentOrFutureAttachment, this.context.realCounts);
  }

  public get canEditFutureEnd(): boolean {
    return this.actionRestrictionsService.canEditFutureEnd(this.currentOrFutureAttachment, this.context.realCounts);
  }

  public get canEditFutureStart(): boolean {
    return this.actionRestrictionsService.canEditFutureStart(this.currentOrFutureAttachment, this.context.realCounts);
  }

  public get canCancelUnlinking(): boolean {
    return this.actionRestrictionsService.canCancelUnlinking(this.currentOrFutureAttachment, this.context.realCounts);
  }

  public get canUnlink(): boolean {
    return this.actionRestrictionsService.canUnlink(this.currentOrFutureAttachment);
  }

  public get canLink(): boolean {
    return this.actionRestrictionsService.canLink(this.currentOrFutureAttachment);
  }

  constructor(
    private actionRestrictionsService: AttachmentsActionRestrictionsService,
    private actionsService: EstablishmentListActionsService,
  ) { }

  public openAttachmentDeletion(): void {
    this.actionsService.openAttachmentDeletion(this.currentOrFutureAttachment?.id);
  }

  public openAttachmentFutureDeactivationEdition(): void {
    this.actionsService.openAttachmentFutureDeactivationEdition(
      this.establishment,
      this.currentOrFutureAttachment,
      this.context.lastCountPeriod,
    );
  }

  public openAttachmentFutureActivationEdition(): void {
    this.actionsService.openAttachmentFutureActivationEdition(
      this.establishment,
      this.currentOrFutureAttachment,
      this.context.lastCountPeriod,
      new Date(this.context.contract.theoricalStartOn),
    );
  }

  public openAttachmentLinking(): void {
    const attachment = this.entry.currentAttachment || this.entry.lastAttachment;

    this.actionsService.openAttachmentLinking(this.establishment, attachment, this.context.contract);
  }

  public openAttachmentUnlinking(): void {
    this.actionsService.openAttachmentUnlinking(
      this.establishment,
      this.currentOrFutureAttachment,
      this.context.lastCountPeriod,
    );
  }

  public openAttachmentUnlinkingCancellation(): void {
    this.actionsService.openAttachmentUnlinkingCancellation(this.establishment, this.currentOrFutureAttachment);
  }

  public openAttachmentExclusion(): void {
    this.actionsService.openAttachmentExclusion(this.establishment, this.context.contract.product);
  }

  public redirectToContract(): void {
    if (!this.currentOrFutureAttachment) {
      return;
    }
    this.actionsService.redirectToContract(this.currentOrFutureAttachment.contractID);
  }
}
