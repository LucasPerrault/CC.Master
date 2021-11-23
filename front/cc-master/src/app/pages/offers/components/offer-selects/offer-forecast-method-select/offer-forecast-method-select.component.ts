import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
} from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { ForecastMethod, forecastMethods } from '../../../enums/forecast-method.enum';

@Component({
  selector: 'cc-offer-forecast-method-select',
  templateUrl: './offer-forecast-method-select.component.html',
  styleUrls: ['offer-forecast-method-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OfferForecastMethodSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: OfferForecastMethodSelectComponent,
    },
  ],
})
export class OfferForecastMethodSelectComponent implements OnInit, OnDestroy, ControlValueAccessor, Validator {
  @Input() public required = false;
  @Input() public placeholder: string;
  @Input() public hideClearer = false;
  @Input() public set disabled(isDisabled: boolean) { this.setDisabledState(isDisabled); }

  public forecastMethods = forecastMethods;

  public formControl: FormControl = new FormControl();
  private destroy$: Subject<void> = new Subject<void>();

  constructor() { }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(forecastMethod => this.onChange(forecastMethod));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (forecastMethod: ForecastMethod) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(forecastMethod: ForecastMethod): void {
    if (!!forecastMethod && this.formControl.value !== forecastMethod) {
      this.formControl.setValue(forecastMethod);
    }
  }

  public setDisabledState(isDisabled: boolean) {
    if (isDisabled) {
      this.formControl.disable();
      return;
    }
    this.formControl.enable();
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formControl.invalid) {
      return { invalid: true };
    }
  }

  public trackBy(index: number, forecastMethod: ForecastMethod): ForecastMethod {
    return forecastMethod;
  }
}
