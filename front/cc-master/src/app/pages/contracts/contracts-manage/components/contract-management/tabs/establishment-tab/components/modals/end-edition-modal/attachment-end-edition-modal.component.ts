import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { ELuDateGranularity } from '@lucca-front/ng/core';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { endOfMonth, isEqual } from 'date-fns';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { getAttachmentEndReason, IAttachmentEndReason } from '../../../constants/attachment-end-reason.const';
import { AttachmentEndEditionConditions } from '../../../services/attachments-action-conditions';
import { EstablishmentsDataService } from '../../../services/establishments-data.service';
import { AttachmentEndEditionModalMode, IAttachmentEndEditionModalData } from './attachment-end-edition-modal-data.interface';

export enum AttachmentEndEditionFormKey {
  EndDate = 'end',
  EndReason = 'endReason',
}

@Component({
  selector: 'cc-attachment-end-edition-modal',
  templateUrl: './attachment-end-edition-modal.component.html',
})
export class AttachmentEndEditionModalComponent implements OnInit, OnDestroy, ILuModalContent {
  public title: string;
  public submitLabel: string;
  public submitDisabled: boolean;

  public formGroup: FormGroup;
  public formKey = AttachmentEndEditionFormKey;

  public mode = AttachmentEndEditionModalMode;
  public min: Date;
  public max: Date;
  public granularity = ELuDateGranularity;

  private destroy$: Subject<void> = new Subject();

  constructor(
    @Inject(LU_MODAL_DATA) public modalData: IAttachmentEndEditionModalData,
    private translatePipe: TranslatePipe,
    private establishmentsDataService: EstablishmentsDataService,
  ) {
    this.title = this.getTitle();
    this.submitLabel = this.translatePipe.transform(this.getSubmitLabel(modalData.mode));
    this.submitDisabled = modalData.mode !== AttachmentEndEditionModalMode.UnlinkingCancellation;

    this.min = AttachmentEndEditionConditions.minDate(this.modalData.lastCountPeriod, this.modalData.attachments);
    this.max = AttachmentEndEditionConditions.maxDate(
      this.modalData.contractCloseOn,
      this.modalData.establishments,
      this.modalData.attachments,
    );

    this.formGroup = new FormGroup({
      [AttachmentEndEditionFormKey.EndDate]: new FormControl(this.getEndDate()),
      [AttachmentEndEditionFormKey.EndReason]: new FormControl(this.getEndReason()),
    });
  }

  public ngOnInit(): void {
    this.formGroup.statusChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.submitDisabled = this.formGroup.invalid);
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public getEndDate(): Date {
    if (!this.modalData.attachments.length || this.modalData.attachments.every(a => !a.end)) {
      return null;
    }

    const endDate = new Date(this.modalData.attachments[0]?.end);
    const areAllEndDateEquals = this.modalData.attachments.every(ce => !!ce.end && isEqual(new Date(ce.end), endDate));
    return areAllEndDateEquals ? endDate : null;
  }

  public getEndReason(): IAttachmentEndReason {
    if (!this.modalData.attachments.length || this.modalData.attachments.every(a => !a.end)) {
      return null;
    }

    const endReason = this.modalData.attachments[0]?.endReason;
    const areAllEndReasonEquals = this.modalData.attachments.every(ce => ce.endReason === endReason);
    return areAllEndReasonEquals ? getAttachmentEndReason(endReason) : null;
  }

  public submitAction(): Observable<void> {
    if (this.modalData.mode === AttachmentEndEditionModalMode.UnlinkingCancellation) {
      const attachmentIds = this.modalData.attachments.map(a => a.id);
      return this.establishmentsDataService.cancelAttachmentUnlinkingRange$(attachmentIds);
    }

    const endDate = endOfMonth(new Date(this.formGroup.get(AttachmentEndEditionFormKey.EndDate).value));
    const endReason = this.formGroup.get(AttachmentEndEditionFormKey.EndReason).value?.id;

    const attachmentIds = this.modalData.attachments.map(ce => ce.id);
    return this.establishmentsDataService.updateAttachmentEndRange$(attachmentIds, endDate, endReason);
  }

  private getTitle(): string {
    const translationKey = this.getTitleTranslationKey(this.modalData.mode);

    const establishments = this.modalData.establishments;
    return this.translatePipe.transform(translationKey, { count: establishments.length });
  }

  private getTitleTranslationKey(mode: AttachmentEndEditionModalMode): string {
    switch (mode) {
      case AttachmentEndEditionModalMode.FutureDeactivationEdition:
        return 'front_contractPage_establishments_endEdition_modal_title';
      case AttachmentEndEditionModalMode.Unlinking:
        return 'front_contractPage_establishments_unlink_modal_title';
      case AttachmentEndEditionModalMode.UnlinkingCancellation:
        return 'front_contractPage_establishments_cancelUnlinking_modal_title';
    }
  }

  private getSubmitLabel(mode: AttachmentEndEditionModalMode): string {
    switch (mode) {
      case AttachmentEndEditionModalMode.FutureDeactivationEdition:
        return 'front_contractPage_establishments_endEdition_modal_button';
      case AttachmentEndEditionModalMode.Unlinking:
        return 'front_contractPage_establishments_unlink_modal_button';
      case AttachmentEndEditionModalMode.UnlinkingCancellation:
        return 'front_contractPage_establishments_cancelUnlinking_modal_button';
    }
  }
}
