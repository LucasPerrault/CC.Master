import { Component, Input } from '@angular/core';

import { EstablishmentType } from '../../constants/establishment-type.enum';
import { IContractEstablishment } from '../../models/contract-establishment.interface';
import { IEstablishmentActionsContext } from '../../models/establishment-actions-context.interface';
import { IEstablishmentAttachment } from '../../models/establishment-attachment.interface';
import { AttachmentsActionRestrictionsService } from '../../services/attachments-action-restrictions.service';
import { EstablishmentListActionsService } from '../../services/establishment-list-actions.service';

@Component({
  selector: 'cc-establishment-list-actions-single',
  templateUrl: './establishment-list-actions-single.component.html',
})
export class EstablishmentListActionsSingleComponent {
  @Input() public establishment: IContractEstablishment;
  @Input() public attachment: IEstablishmentAttachment;
  @Input() public context: IEstablishmentActionsContext;

  public etsType = EstablishmentType;

  public get canDelete(): boolean {
    return this.actionRestrictionsService.canDelete(this.attachment, this.context.realCounts);
  }

  public get canEditFutureEnd(): boolean {
    return this.actionRestrictionsService.canEditFutureEnd(this.attachment, this.context.realCounts);
  }

  public get canEditFutureStart(): boolean {
    return this.actionRestrictionsService.canEditFutureStart(this.attachment, this.context.realCounts);
  }

  public get canCancelUnlinking(): boolean {
    return this.actionRestrictionsService.canCancelUnlinking(this.attachment, this.context.realCounts);
  }

  public get canUnlink(): boolean {
    return this.actionRestrictionsService.canUnlink(this.attachment);
  }

  public get canLink(): boolean {
    return this.actionRestrictionsService.canLink(this.attachment);
  }

  constructor(
    private actionRestrictionsService: AttachmentsActionRestrictionsService,
    private actionsService: EstablishmentListActionsService,
  ) { }

  public openAttachmentDeletion(): void {
    this.actionsService.openAttachmentDeletion(this.attachment?.id);
  }

  public openAttachmentFutureDeactivationEdition(): void {
    this.actionsService.openAttachmentFutureDeactivationEdition(this.establishment, this.attachment, this.context.lastCountPeriod);
  }

  public openAttachmentFutureActivationEdition(): void {
    this.actionsService.openAttachmentFutureActivationEdition(
      this.establishment,
      this.attachment,
      this.context.lastCountPeriod,
      new Date(this.context.contract.theoricalStartOn),
    );
  }

  public openAttachmentLinking(): void {
    this.actionsService.openAttachmentLinking(this.establishment, this.context.contract);
  }

  public openAttachmentUnlinking(): void {
    this.actionsService.openAttachmentUnlinking(
      this.establishment,
      this.attachment,
      this.context.lastCountPeriod,
    );
  }

  public openAttachmentUnlinkingCancellation(): void {
    this.actionsService.openAttachmentUnlinkingCancellation(this.establishment, this.attachment);
  }

  public openAttachmentExclusion(): void {
    this.actionsService.openAttachmentExclusion(this.establishment, this.context.contract.product);
  }

  public redirectToContract(): void {
    if (!this.attachment) {
      return;
    }
    this.actionsService.redirectToContract(this.attachment.contractID);
  }
}
