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

import { currencies, IOfferCurrency } from '../../../models/offer-currency.interface';

@Component({
  selector: 'cc-offer-currency-api-select',
  templateUrl: './offer-currency-select.component.html',
  styleUrls: ['./offer-currency-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OfferCurrencySelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: OfferCurrencySelectComponent,
    },
  ],
})
export class OfferCurrencySelectComponent implements OnInit, OnDestroy, ControlValueAccessor, Validator {
  @Input() public placeholder: string;
  @Input() public multiple = false;
  @Input() public required = false;
  @Input() public hideClearer = false;
  @Input() public set disabled(isDisabled: boolean) { this.setDisabledState(isDisabled); }

  public formControl: FormControl = new FormControl();
  public currencies: IOfferCurrency[];

  private destroy$: Subject<void> = new Subject<void>();

  constructor() {
    this.currencies = currencies;
  }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(currency => this.onChange(currency));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (currency: IOfferCurrency | IOfferCurrency[]) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(currency: IOfferCurrency | IOfferCurrency[]): void {
    if (!!currency && this.formControl.value !== currency) {
      this.formControl.setValue(currency);
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

  public trackBy(index: number, currency: IOfferCurrency): number {
    return currency.code;
  }
}
