import { Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
} from '@angular/forms';
import { IRange, rangeValidatorFn } from '@cc/common/forms/validators/range-validator';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'cc-minimal-billing-percentage',
  templateUrl: './minimal-billing-percentage.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => MinimalBillingPercentageComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: MinimalBillingPercentageComponent,
    },
  ],
})
export class MinimalBillingPercentageComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {
  @Input() public textfieldClass?: string;

  @Input()
  public set disabled(isDisabled: boolean) {
    this.setDisabledState(isDisabled);
  }

  @Input()
  public get min(): number { return this.range.min; };
  public set min(value: number) {
    this.range.min = value;
    this.updateFormValidators(value, this.range.max);
  }

  @Input()
  public get max(): number { return this.range.max; };
  public set max(value: number) {
    this.range.max = value;
    this.updateFormValidators(this.range.min, value);
  }

  public get hasRangeError(): boolean {
    return this.minimalBilling.hasError('range');
  }

  public onChange: (minimalBilling: number) => void;
  public onTouch: () => void;

  public minimalBilling: FormControl;
  private range: IRange = {
    min: 0,
    max: 100,
  };

  private destroy$: Subject<void> = new Subject<void>();

  constructor() {
    this.minimalBilling = new FormControl(null, rangeValidatorFn(this.range));
  }

  public ngOnInit(): void {
    this.minimalBilling.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(m => this.onChange(m));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(minimalBilling: number): void {
    if (minimalBilling !== this.minimalBilling.value && minimalBilling !== null) {
      this.minimalBilling.setValue(minimalBilling, { emitEvent: false });
    }
  }

  public setDisabledState(isDisabled: boolean) {
    if (isDisabled) {
      this.minimalBilling.disable();
      return;
    }
    this.minimalBilling.enable();
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.minimalBilling.invalid) {
      return { invalid: true };
    }
  }

  private updateFormValidators(min: number, max: number): void {
    this.minimalBilling.setValidators(rangeValidatorFn({ min, max }));
  }
}
