import { Component, Input } from '@angular/core';

import { IContractEstablishment } from '../../models/contract-establishment.interface';
import { IEstablishmentActionsContext } from '../../models/establishment-actions-context.interface';
import { IEstablishmentAttachment } from '../../models/establishment-attachment.interface';
import { IListEntry, ListEntryType } from '../../models/establishment-list-entry.interface';
import { AttachmentsActionRestrictionsService } from '../../services/attachments-action-restrictions.service';
import { EstablishmentListActionsService } from '../../services/establishment-list-actions.service';

@Component({
  selector: 'cc-establishment-list-actions-multiple',
  templateUrl: './establishment-list-actions-multiple.component.html',
})
export class EstablishmentListActionsMultipleComponent {
  @Input() public type: ListEntryType;
  @Input() public context: IEstablishmentActionsContext;
  @Input() public entries: IListEntry[];

  public get establishments(): IContractEstablishment[] {
    return this.entries.map(e => e.establishment);
  }

  public get attachments(): IEstablishmentAttachment[] {
    return this.entries.map(e => e.attachment);
  }

  public listEntryType = ListEntryType;

  public get isDisabled(): boolean {
    return !this.establishments.length;
  }

  public get canDelete(): boolean {
    return this.actionRestrictionsService.canDeleteRange(this.attachments, this.context.realCounts);
  }

  public get canEditFutureEnd(): boolean {
    return this.actionRestrictionsService.canEditFutureEndRange(this.attachments, this.context.realCounts);
  }

  public get canEditFutureStart(): boolean {
    return this.actionRestrictionsService.canEditFutureStartRange(this.attachments, this.context.realCounts);
  }

  public get canUnlink(): boolean {
    return this.actionRestrictionsService.canUnlinkRange(this.attachments);
  }

  public get canLink(): boolean {
    return this.actionRestrictionsService.canLinkRange(this.attachments);
  }

  constructor(
    private actionRestrictionsService: AttachmentsActionRestrictionsService,
    private actionsService: EstablishmentListActionsService,
  ) { }

  public openAttachmentDeletion(): void {
    const attachmentIds = this.attachments.map(a => a.id);
    this.actionsService.openAttachmentsDeletion(attachmentIds);
  }

  public openAttachmentFutureDeactivationEdition(): void {
    this.actionsService.openAttachmentsFutureDeactivationEdition(
      this.establishments,
      this.attachments,
      this.context.lastCountPeriod,
    );
  }

  public openAttachmentFutureActivationEdition(): void {
    this.actionsService.openAttachmentsFutureActivationEdition(
      this.establishments,
      this.attachments,
      this.context.lastCountPeriod,
      new Date(this.context.contract.theoricalStartOn),
    );
  }

  public openAttachmentLinking(): void {
    this.actionsService.openAttachmentsLinking(this.establishments, this.attachments, this.context.contract);
  }

  public openAttachmentUnlinking(): void {
    this.actionsService.openAttachmentsUnlinking(this.establishments, this.attachments, this.context.lastCountPeriod);
  }

  public openAttachmentExclusion(): void {
    this.actionsService.openAttachmentsExclusion(this.establishments, this.context.contract.product);
  }
}
