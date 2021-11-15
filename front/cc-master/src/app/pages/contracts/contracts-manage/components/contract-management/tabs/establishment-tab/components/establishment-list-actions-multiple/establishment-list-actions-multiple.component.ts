import { Component, Input } from '@angular/core';

import { EstablishmentType } from '../../constants/establishment-type.enum';
import { IContractEstablishment } from '../../models/contract-establishment.interface';
import { IEstablishmentActionsContext } from '../../models/establishment-actions-context.interface';
import { IEstablishmentAttachment } from '../../models/establishment-attachment.interface';
import { IEstablishmentWithAttachments } from '../../models/establishment-with-attachments.interface';
import { AttachmentsActionRestrictionsService } from '../../services/attachments-action-restrictions.service';
import { EstablishmentListActionsService } from '../../services/establishment-list-actions.service';

@Component({
  selector: 'cc-establishment-list-actions-multiple',
  templateUrl: './establishment-list-actions-multiple.component.html',
})
export class EstablishmentListActionsMultipleComponent {
  @Input() public context: IEstablishmentActionsContext;
  @Input() public entries: IEstablishmentWithAttachments[];

  public get establishments(): IContractEstablishment[] {
    return this.entries.map(e => e.establishment);
  }

  public get currentOrFutureAttachments(): IEstablishmentAttachment[] {
    return this.entries.map(e => e.currentAttachment || e.nextAttachment);
  }

  public etsType = EstablishmentType;

  public get isDisabled(): boolean {
    return !this.establishments.length;
  }

  public get canDelete(): boolean {
    return this.actionRestrictionsService.canDeleteRange(this.currentOrFutureAttachments, this.context.realCounts);
  }

  public get canEditFutureEnd(): boolean {
    return this.actionRestrictionsService.canEditFutureEndRange(this.currentOrFutureAttachments, this.context.realCounts);
  }

  public get canEditFutureStart(): boolean {
    return this.actionRestrictionsService.canEditFutureStartRange(this.currentOrFutureAttachments, this.context.realCounts);
  }

  public get canUnlink(): boolean {
    return this.actionRestrictionsService.canUnlinkRange(this.currentOrFutureAttachments);
  }

  public get canLink(): boolean {
    return this.actionRestrictionsService.canLinkRange(this.currentOrFutureAttachments);
  }

  constructor(
    private actionRestrictionsService: AttachmentsActionRestrictionsService,
    private actionsService: EstablishmentListActionsService,
  ) { }

  public openAttachmentDeletion(): void {
    const attachmentIds = this.currentOrFutureAttachments.map(a => a.id);
    this.actionsService.openAttachmentsDeletion(attachmentIds);
  }

  public openAttachmentFutureDeactivationEdition(): void {
    this.actionsService.openAttachmentsFutureDeactivationEdition(
      this.establishments,
      this.currentOrFutureAttachments,
      this.context.lastCountPeriod,
    );
  }

  public openAttachmentFutureActivationEdition(): void {
    this.actionsService.openAttachmentsFutureActivationEdition(
      this.establishments,
      this.currentOrFutureAttachments,
      this.context.lastCountPeriod,
      new Date(this.context.contract.theoricalStartOn),
    );
  }

  public openAttachmentLinking(): void {
    const finishedAttachments = this.entries.map(e => e.lastAttachment);
    const allAttachments = [...this.currentOrFutureAttachments, ...finishedAttachments];

    this.actionsService.openAttachmentsLinking(this.establishments, allAttachments, this.context.contract);
  }

  public openAttachmentUnlinking(): void {
    this.actionsService.openAttachmentsUnlinking(this.establishments, this.currentOrFutureAttachments, this.context.lastCountPeriod);
  }

  public openAttachmentExclusion(): void {
    this.actionsService.openAttachmentsExclusion(this.establishments, this.context.contract.product);
  }
}
