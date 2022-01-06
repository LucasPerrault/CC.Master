import { Component, forwardRef, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, FormGroup, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { DistributorFilter } from '../../../common/distributor-filter-button-group';
import { IContractsDraftFilter } from '../../models';

enum DraftFilterKey {
  SaleType = 'saleType',
  DraftName = 'draftName',
}

@Component({
  selector: 'cc-contracts-draft-filter',
  templateUrl: './contracts-draft-filter.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ContractsDraftFilterComponent),
      multi: true,
    },
  ],
})
export class ContractsDraftFilterComponent implements OnInit, OnDestroy, ControlValueAccessor {

  public formGroup: FormGroup;
  public formKey = DraftFilterKey;

  private destroy$: Subject<void> = new Subject();

  constructor() {
    this.formGroup = new FormGroup({
      [DraftFilterKey.SaleType]: new FormControl(DistributorFilter.All),
      [DraftFilterKey.DraftName]: new FormControl(),
    });
  }

  public ngOnInit(): void {
    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(form => this.onChange(form));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (draftsFilter: IContractsDraftFilter) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(form: IContractsDraftFilter): void {
    if (!!form && form !== this.formGroup.value) {
      this.formGroup.setValue(form);
    }
  }
}
