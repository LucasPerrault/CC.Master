import { Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { ALuApiService } from '@lucca-front/ng/api';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { OfferPriceListApiSelectService } from './offer-price-list-api-select.service';
import { IPriceListOfferSelectOption } from './offer-price-list-selection.interface';

@Component({
  selector: 'cc-offer-price-list-api-select',
  templateUrl: './offer-price-list-api-select.component.html',
  styleUrls: ['./offer-price-list-api-select.component.scss'],
  providers: [
    {
      provide: ALuApiService,
      useClass: OfferPriceListApiSelectService,
    },
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OfferPriceListApiSelectComponent),
      multi: true,
    },
  ],
})
export class OfferPriceListApiSelectComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() placeholder: string;

  public formControl: FormControl = new FormControl();
  private destroy$: Subject<void> = new Subject();

  constructor() {}

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(offer => this.onChange(offer));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (offer: IPriceListOfferSelectOption) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(offer: IPriceListOfferSelectOption): void {
    this.formControl.setValue(offer);
  }

  public trackBy(index: number, offer: IPriceListOfferSelectOption): number {
    return offer.id;
  }
}
