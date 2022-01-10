import { Component, Input } from '@angular/core';

import { IEstablishmentActionsContext } from '../../models/establishment-actions-context.interface';
import { IListEntry, ListEntryType } from '../../models/establishment-list-entry.interface';
import { AttachmentsActionRestrictionsService } from '../../services/attachments-action-restrictions.service';
import { EstablishmentListActionsService } from '../../services/establishment-list-actions.service';

@Component({
  selector: 'cc-establishment-list-actions-single',
  templateUrl: './establishment-list-actions-single.component.html',
})
export class EstablishmentListActionsSingleComponent {
  @Input() public type: ListEntryType;
  @Input() public entry: IListEntry;
  @Input() public context: IEstablishmentActionsContext;

  public listEntryType = ListEntryType;

  public get canDelete(): boolean {
    return this.actionRestrictionsService.canDelete(this.entry.attachment, this.context.realCounts);
  }

  public get canEditFutureEnd(): boolean {
    return this.actionRestrictionsService.canEditFutureEnd(this.entry.attachment, this.context.realCounts);
  }

  public get canEditFutureStart(): boolean {
    return this.actionRestrictionsService.canEditFutureStart(this.entry.attachment, this.context.realCounts);
  }

  public get canCancelUnlinking(): boolean {
    return this.actionRestrictionsService.canCancelUnlinking(this.entry.attachment, this.context.realCounts);
  }

  public get canUnlink(): boolean {
    return this.actionRestrictionsService.canUnlink(this.entry.attachment);
  }

  public get canLink(): boolean {
    return this.actionRestrictionsService.canLink(this.entry.attachment);
  }

  constructor(
    private actionRestrictionsService: AttachmentsActionRestrictionsService,
    private actionsService: EstablishmentListActionsService,
  ) { }

  public openAttachmentDeletion(): void {
    this.actionsService.openAttachmentDeletion(this.entry.attachment?.id);
  }

  public openAttachmentFutureDeactivationEdition(): void {
    this.actionsService.openAttachmentFutureDeactivationEdition(
      this.entry.establishment,
      this.entry.attachment,
      this.context.lastCountPeriod,
    );
  }

  public openAttachmentFutureActivationEdition(): void {
    this.actionsService.openAttachmentFutureActivationEdition(
      this.entry.establishment,
      this.entry.attachment,
      this.context.lastCountPeriod,
      new Date(this.context.contract.theoricalStartOn),
    );
  }

  public openAttachmentLinking(): void {
    this.actionsService.openAttachmentLinking(this.entry.establishment, this.entry.attachment, this.context.contract);
  }

  public openAttachmentUnlinking(): void {
    this.actionsService.openAttachmentUnlinking(
      this.entry.establishment,
      this.entry.attachment,
      this.context.lastCountPeriod,
    );
  }

  public openAttachmentUnlinkingCancellation(): void {
    this.actionsService.openAttachmentUnlinkingCancellation(this.entry.establishment, this.entry.attachment);
  }

  public openAttachmentExclusion(): void {
    this.actionsService.openAttachmentExclusion(this.entry.establishment, this.context.contract.product);
  }

  public redirectToContract(): void {
    if (!this.entry.attachment) {
      return;
    }
    this.actionsService.redirectToContract(this.entry.attachment.contractID);
  }
}
