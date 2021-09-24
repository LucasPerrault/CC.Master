import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { ALuApiService } from '@lucca-front/ng/api';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { IOfferCurrency } from '../../../models/offer-currency.interface';
import { OfferCurrencyApiSelectService } from './offer-currency-api-select.service';

@Component({
  selector: 'cc-offer-currency-api-select',
  templateUrl: './offer-currency-api-select.component.html',
  styleUrls: ['./offer-currency-api-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: ALuApiService,
      useClass: OfferCurrencyApiSelectService,
    },
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OfferCurrencyApiSelectComponent),
      multi: true,
    },
  ],
})
export class OfferCurrencyApiSelectComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() public placeholder: string;
  public formControl: FormControl = new FormControl();

  private destroy$: Subject<void> = new Subject<void>();

  constructor() { }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(currency => this.onChange(currency));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (currency: IOfferCurrency) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(currency: IOfferCurrency): void {
    if (!!currency && this.formControl.value !== currency) {
      this.formControl.setValue(currency);
    }
  }

  public trackBy(index: number, currency: IOfferCurrency): number {
    return currency.code;
  }
}
