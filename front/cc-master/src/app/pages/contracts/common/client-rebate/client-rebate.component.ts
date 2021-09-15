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
import { endOfMonth } from 'date-fns';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { IClientRebate } from './client-rebate.interface';

@Component({
  selector: 'cc-client-rebate',
  templateUrl: './client-rebate.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ClientRebateComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: ClientRebateComponent,
    },
  ],
})
export class ClientRebateComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {
  @Input() public textfieldClass?: string;
  @Input()
  public set disabled(isDisabled: boolean) {
    this.setDisabledState(isDisabled);
  }

  public onChange: (clientRebate: IClientRebate) => void;
  public onTouch: () => void;

  public get range(): IRange {
    return { min: 0, max: 100 };
  }

  public get hasRangeError(): boolean {
    return this.clientRebateCount.hasError('range');
  }

  public clientRebate: IClientRebate;
  public clientRebateCount: FormControl;
  public endClientRebateAt: FormControl;

  private destroy$: Subject<void> = new Subject<void>();

  constructor() {
    this.clientRebateCount = new FormControl(null, [rangeValidatorFn(this.range)]);
    this.endClientRebateAt = new FormControl(null);
    this.clientRebate = {
      count: this.clientRebateCount.value,
      endAt: this.endClientRebateAt.value,
    };
  }

  public ngOnInit(): void {
    this.clientRebateCount.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(c => this.updateCount(c));

    this.endClientRebateAt.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(e => this.updateEndAt(e));
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

  public writeValue(clientRebate: IClientRebate): void {
    if (!clientRebate) {
      return;
    }

    if (clientRebate.endAt !== this.endClientRebateAt.value) {
      this.endClientRebateAt.setValue(clientRebate.endAt, { emitEvent: false });
    }

    if (clientRebate.count !== this.clientRebateCount.value) {
      this.clientRebateCount.setValue(clientRebate.count, { emitEvent: false });
    }
  }

  public setDisabledState(isDisabled: boolean) {
    if (isDisabled) {
      this.clientRebateCount.disable();
      this.endClientRebateAt.disable();
      return;
    }
    this.clientRebateCount.enable();
    this.endClientRebateAt.enable();
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.clientRebateCount.invalid || this.endClientRebateAt.invalid) {
      return  { invalid: true };
    }
  }

  public updateCount(count: number): void {
    this.clientRebate.count = count;
    this.onChange(this.clientRebate);
  }

  public updateEndAt(date: Date): void {
    this.clientRebate.endAt = endOfMonth(date);
    this.onChange(this.clientRebate);
  }
}
