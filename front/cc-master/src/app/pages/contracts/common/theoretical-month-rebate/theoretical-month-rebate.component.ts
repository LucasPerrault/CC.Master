import { Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
  Validators,
} from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'cc-theoretical-month-rebate',
  templateUrl: './theoretical-month-rebate.component.html',
  styleUrls: ['./theoretical-month-rebate.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TheoreticalMonthRebateComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: TheoreticalMonthRebateComponent,
    },
  ],
})
export class TheoreticalMonthRebateComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {
  @Input() public textfieldClass?: string;
  @Input()
  public set disabled(isDisabled: boolean) {
    this.setDisabledState(isDisabled);
  }

  public onChange: (theoreticalMonthRebate: number) => void;
  public onTouch: () => void;

  public theoreticalMonthRebate: FormControl;

  public get hasRequiredError(): boolean {
    return this.theoreticalMonthRebate.touched && this.theoreticalMonthRebate.hasError('required');
  }

  public get hasMinError(): boolean {
    return this.theoreticalMonthRebate.hasError('min');
  }

  private destroy: Subject<void> = new Subject<void>();

  constructor() {
    this.theoreticalMonthRebate = new FormControl(null, [Validators.min(0), Validators.required]);
  }

  public ngOnInit(): void {
    this.theoreticalMonthRebate.valueChanges
      .pipe(takeUntil(this.destroy))
      .subscribe(r => this.onChange(r));
  }

  public ngOnDestroy(): void {
    this.destroy.next();
    this.destroy.complete();
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(theoreticalMonthRebate: number): void {
    if (theoreticalMonthRebate !== this.theoreticalMonthRebate.value) {
      this.theoreticalMonthRebate.setValue(theoreticalMonthRebate, { emitEvent: false });
    }
  }

  public setDisabledState(isDisabled: boolean) {
    if (isDisabled) {
      this.theoreticalMonthRebate.disable();
      return;
    }
    this.theoreticalMonthRebate.enable();
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.hasMinError || this.hasRequiredError) {
      return { invalid: true };
    }
  }
}

