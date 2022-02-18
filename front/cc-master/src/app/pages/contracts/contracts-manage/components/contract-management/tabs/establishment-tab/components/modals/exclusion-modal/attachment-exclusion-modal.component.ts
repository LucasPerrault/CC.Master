import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { ISolution } from '@cc/domain/billing/offers';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { EstablishmentsDataService } from '../../../services/establishments-data.service';
import { IAttachmentExclusionModalData } from './attachment-exclusion-modal-data.interface';

@Component({
  selector: 'cc-attachment-exclusion-modal',
  templateUrl: './attachment-exclusion-modal.component.html',
  styleUrls: ['./attachment-exclusion-modal.component.scss'],
})
export class AttachmentExclusionModalComponent implements ILuModalContent, OnInit, OnDestroy {
  public title: string;
  public submitDisabled = true;
  public submitLabel = this.translatePipe.transform('front_contractPage_establishments_exclude_modal_button');

  public get hasMultiSolutions(): boolean {
    return this.modalData?.product?.solutions?.length > 1;
  }

  public selectedSolutions: FormControl = new FormControl();

  private destroy$ = new Subject<void>();

  constructor(
    @Inject(LU_MODAL_DATA) public modalData: IAttachmentExclusionModalData,
    private translatePipe: TranslatePipe,
    private establishmentsDataService: EstablishmentsDataService,
  ) {
    this.title = this.getTitle();
  }

  public ngOnInit(): void {
    this.selectedSolutions.statusChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.submitDisabled = this.selectedSolutions.invalid);

    this.selectedSolutions.setValue(this.modalData.product.solutions);
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public submitAction(): Observable<void> {
    const establishmentIds = this.modalData.establishments.map(e => e.id);
    const solutionIds = this.selectedSolutions.value?.map(s => s.id) ?? [];
    return this.establishmentsDataService.excludeEstablishmentRange$(establishmentIds, solutionIds);
  }

  public getDescription(): string {
    if (this.hasMultiSolutions) {
      return this.translatePipe.transform('establishments_exclude_modal_multi_solutions_description', {
        count: this.modalData.establishments.length,
      });
    }

    return this.translatePipe.transform('establishments_exclude_modal_description', {
      count: this.modalData.establishments.length,
      solutionName: this.modalData?.product?.solutions[0]?.name,
    });
  }

  public getSolutionNames(solutions: ISolution[]): string {
    return solutions?.map(s => s.name)?.join(', ') ?? '';
  }

  private getTitle(): string {
    return this.translatePipe.transform('front_contractPage_establishments_exclude_modal_title', {
      count: this.modalData.establishments.length,
      name: this.modalData.establishments[0]?.name,
    });
  }

}
