import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { ELuDateGranularity } from '@lucca-front/ng/core';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { differenceInMonths, startOfMonth } from 'date-fns';
import { Observable, Subject } from 'rxjs';
import { debounceTime, takeUntil } from 'rxjs/operators';

import { IContractEstablishment } from '../../../models/contract-establishment.interface';
import { IEstablishmentExcludedEntity } from '../../../models/establishment-excluded-entity.interface';
import { AttachmentLinkingConditions } from '../../../services/attachments-action-conditions';
import { EstablishmentsDataService } from '../../../services/establishments-data.service';
import { IAttachmentLinkingModalData } from './attachment-linking-modal-data.interface';

enum AttachmentLinkingFormKey {
  StartDate = 'startDate',
  MonthRebate = 'monthRebate',
}

@Component({
  selector: 'cc-attachment-linking-modal',
  templateUrl: './attachment-linking-modal.component.html',
})
export class AttachmentLinkingModalComponent implements OnInit, OnDestroy, ILuModalContent {
  public title: string;
  public submitLabel: string;
  public submitDisabled = true;

  public formGroup: FormGroup;
  public formKey = AttachmentLinkingFormKey;

  public min: Date;
  public max: Date;
  public granularity = ELuDateGranularity;

  private destroy$: Subject<void> = new Subject();

  constructor(
    @Inject(LU_MODAL_DATA) public modalData: IAttachmentLinkingModalData,
    private translatePipe: TranslatePipe,
    private establishmentsDataService: EstablishmentsDataService,
  ) {
    this.submitLabel = this.translatePipe.transform('front_contractPage_establishments_linking_modal_button');
    this.title = this.getTitle();

    this.min = AttachmentLinkingConditions.minDate(this.modalData.establishments, this.modalData?.contract?.theoricalStartOn);
    this.max = AttachmentLinkingConditions.maxDate(this.modalData?.contract?.closeOn);

    this.formGroup = new FormGroup({
      [AttachmentLinkingFormKey.StartDate]: new FormControl(),
      [AttachmentLinkingFormKey.MonthRebate]: new FormControl(this.modalData.contract.nbMonthTheorical),
    });
  }

  public ngOnInit(): void {
    this.formGroup.get(AttachmentLinkingFormKey.StartDate).valueChanges
      .pipe(takeUntil(this.destroy$), debounceTime(300))
      .subscribe(startDate => this.updateMonthRebate(new Date(startDate)));

    this.formGroup.statusChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.submitDisabled = this.formGroup.invalid);
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public submitAction(): Observable<void> {
    const contractId = this.modalData.contract.id;
    const startDate = startOfMonth(new Date(this.formGroup.get(AttachmentLinkingFormKey.StartDate).value));
    const monthRebate = this.formGroup.get(AttachmentLinkingFormKey.MonthRebate).value;

    const establishmentIds = this.modalData.establishments.map(e => e.id);
    const excludedEntities = this.getExcludedEntitiesToDelete(this.modalData.establishments);
    return this.establishmentsDataService.createAttachmentRange$(contractId, establishmentIds, startDate, monthRebate, excludedEntities);
  }

  private getTitle(): string {
    return this.translatePipe.transform('front_contractPage_establishments_linking_modal_title', {
      count: this.modalData.establishments.length,
    });
  }

  private updateMonthRebate(dateSelected: Date): void {
    const contractStartDate = new Date(this.modalData.contract.theoricalStartOn);
    const months = differenceInMonths(dateSelected, contractStartDate);
    const monthRebateCount = 0 <= months && months <= this.modalData.contract.nbMonthTheorical
      ? this.modalData.contract.nbMonthTheorical - months
      : 0;
    this.formGroup.get(AttachmentLinkingFormKey.MonthRebate).patchValue(monthRebateCount);
  }

  private getExcludedEntitiesToDelete(establishments: IContractEstablishment[]): IEstablishmentExcludedEntity[] {
    return establishments
      .map(e => e.excludedEntities)
      .reduce((flattened, values) => [...flattened, ...values])
      .filter(e => e.productId === this.modalData.contract.productId);
  }
}
