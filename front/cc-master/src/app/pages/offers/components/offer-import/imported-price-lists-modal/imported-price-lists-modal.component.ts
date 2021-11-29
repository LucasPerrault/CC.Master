import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { ELuDateGranularity } from '@lucca-front/ng/core';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { IPriceListForm } from '../../../models/price-list-form.interface';
import { PriceListsValidators,PriceListValidationError } from '../../../services/price-lists.validators';
import { IImportedPriceListsModalData } from './imported-price-lists-modal-data.interface';

enum ImportedListFormKey {
  StartsOn = 'startsOn',
  PriceRows = 'rows',
}

@Component({
  selector: 'cc-imported-price-lists-modal',
  templateUrl: './imported-price-lists-modal.component.html',
  styleUrls: ['./imported-price-lists-modal.component.scss'],
})
export class ImportedPriceListsModalComponent implements OnInit, OnDestroy, ILuModalContent {
  public title = this.translatePipe.transform('offers_form_imported_priceList_title', { count: this.data.priceLists?.length });
  public submitLabel = this.translatePipe.transform('offers_form_imported_priceList_button');
  public submitDisabled = true;

  public formArrayKey = 'importedPriceLists';
  public formArray: FormArray = new FormArray([], [PriceListsValidators.uniqStartsOnRange]);
  public formKey = ImportedListFormKey;
  public formGroup: FormGroup = new FormGroup(
    { [this.formArrayKey]: this.formArray },
  );
  public granularity = ELuDateGranularity;
  public validationError = PriceListValidationError;

  private destroy$: Subject<void> = new Subject();

  constructor(
    @Inject(LU_MODAL_DATA) public data: IImportedPriceListsModalData,
    private translatePipe: TranslatePipe,
  ) {
    this.addRange(data.priceLists);
  }

  public ngOnInit(): void {
    this.formArray.statusChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.submitDisabled = this.formArray.invalid);
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public hasErrors(control: AbstractControl): boolean {
    return this.formArray.hasError(PriceListValidationError.UniqStartsOn)
      || control.hasError(PriceListValidationError.StartsOnFirstDayOfTheMonth);
  }

  public trackBy(index: number, list: AbstractControl): AbstractControl {
    return list;
  }

  public submitAction(): IPriceListForm[] {
    return this.formArray.value;
  }

  private addRange(lists: IPriceListForm[]): void {
    lists.forEach(list => this.add(list));
  }

  private add(list: IPriceListForm): void {
    const formGroup = this.getFormGroup(list);
    this.formArray.push(formGroup);
  }

  private getFormGroup(list: IPriceListForm): FormGroup {
    return new FormGroup(
      {
        [ImportedListFormKey.StartsOn]: new FormControl(new Date(list?.startsOn)),
        [ImportedListFormKey.PriceRows]: new FormControl(list?.rows),
      },
      [PriceListsValidators.startsOnFirstDayOfTheMonth, PriceListsValidators.validPrices]);
  }
}
