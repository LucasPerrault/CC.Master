import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { ELuDateGranularity } from '@lucca-front/ng/core';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { addMonths, isEqual, startOfMonth } from 'date-fns';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { EstablishmentsDataService } from '../../../services/establishments-data.service';
import { IAttachmentStartEditionModalData } from './attachment-start-edition-modal-data.interface';

enum AttachmentStartEditionFormKey {
  StartDate = 'startDate',
  MonthRebate = 'monthRebate',
}

@Component({
  selector: 'cc-attachment-start-edition-modal',
  templateUrl: './attachment-start-edition-modal.component.html',
})
export class AttachmentStartEditionModalComponent implements OnInit, OnDestroy, ILuModalContent {
  public title: string;
  public submitLabel: string;
  public submitDisabled = true;

  public formGroup: FormGroup;
  public formKey = AttachmentStartEditionFormKey;

  public granularity = ELuDateGranularity;

  public get min(): Date {
    return !!this.modalData.lastCountPeriod
      ? addMonths(this.modalData.lastCountPeriod, 1)
      : this.getLastAttachmentStartDate();
  }

  private destroy$: Subject<void> = new Subject();

  constructor(
    @Inject(LU_MODAL_DATA) public modalData: IAttachmentStartEditionModalData,
    private translatePipe: TranslatePipe,
    private establishmentsDataService: EstablishmentsDataService,
  ) {
    this.submitLabel = this.translatePipe.transform('front_contractPage_establishments_startEdition_modal_button');
    this.title = this.getTitle();

    this.formGroup = new FormGroup({
      [AttachmentStartEditionFormKey.StartDate]: new FormControl(this.getStartDate()),
      [AttachmentStartEditionFormKey.MonthRebate]: new FormControl(this.getMonthFreeCount()),
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

  public getStartDate(): Date {
    if (!this.modalData.attachments || !this.modalData.attachments.length) {
      return null;
    }

    const startDate = new Date(this.modalData.attachments[0]?.start);
    const areAllStartDateEquals = this.modalData.attachments.every(ce => !!ce.start && isEqual(new Date(ce.start), startDate));
    return areAllStartDateEquals ? startDate : null;
  }

  public getMonthFreeCount(): number {
    if (!this.modalData.attachments || !this.modalData.attachments.length) {
      return null;
    }

    const nbMonthFree = this.modalData.attachments[0]?.nbMonthFree;
    const areAllMonthFreeEquals = this.modalData.attachments.every(ce => ce.nbMonthFree === nbMonthFree);
    return areAllMonthFreeEquals ? nbMonthFree : null;
  }

  public submitAction(): Observable<void> {
    const startDate = startOfMonth(new Date(this.formGroup.get(AttachmentStartEditionFormKey.StartDate).value));
    const monthFreeCount = this.formGroup.get(AttachmentStartEditionFormKey.MonthRebate).value;

    const attachmentIds = this.modalData.attachments.map(ce => ce.id);
    return this.establishmentsDataService.updateAttachmentStartRange$(attachmentIds, startDate, monthFreeCount);
  }

  private getTitle(): string {
    const translationKey = 'front_contractPage_establishments_startEdition_modal_title';

    const establishments = this.modalData.establishments;
    return this.translatePipe.transform(translationKey, { count: establishments.length });
  }

  private getLastAttachmentStartDate(): Date {
    if (!this.modalData.attachments?.length) {
      return null;
    }

    const sortedStartedAttachments = this.modalData.attachments
      .sort((a, b) => new Date(a.start).getTime() - new Date(b.start).getTime());

    return new Date(sortedStartedAttachments[0]?.start);
  }
}
