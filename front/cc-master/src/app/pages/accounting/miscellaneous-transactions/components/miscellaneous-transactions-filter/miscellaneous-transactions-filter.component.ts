import { ChangeDetectionStrategy, Component, forwardRef, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormControl, FormGroup, NG_VALUE_ACCESSOR, ValidationErrors } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { IMiscTransactionsFilter, MiscTransactionsFilterKey } from '../../models/misc-transactions-filter.interface';

@Component({
  selector: 'cc-miscellaneous-transactions-filter',
  templateUrl: './miscellaneous-transactions-filter.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => MiscellaneousTransactionsFilterComponent),
      multi: true,
    },
  ],
})
export class MiscellaneousTransactionsFilterComponent implements OnInit, OnDestroy, ControlValueAccessor {

  public formGroup: FormGroup;
  public formKey = MiscTransactionsFilterKey;

  private destroy$: Subject<void> = new Subject();

  constructor() {
    this.formGroup = new FormGroup({
      [MiscTransactionsFilterKey.Contracts]: new FormControl([]),
      [MiscTransactionsFilterKey.BillingEntities]: new FormControl([]),
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

  public onChange: (filter: IMiscTransactionsFilter) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(filter: IMiscTransactionsFilter): void {
    if (!!filter && filter !== this.formGroup.value) {
      this.formGroup.patchValue(filter);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formGroup.invalid) {
      return { invalid: true };
    }
  }

}
