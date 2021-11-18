import { ChangeDetectionStrategy, Component, forwardRef, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl,
  FormGroup,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
} from '@angular/forms';
import { SelectDisplayMode } from '@cc/common/forms';
import { ELuDateGranularity } from '@lucca-front/ng/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { PriceListsTimelineService } from '../../../services/price-lists-timeline.service';
import { IOfferCreationForm } from './offer-creation-form.interface';

enum OfferFormKey {
  Name = 'name',
  Product = 'product',
  BillingUnit = 'billingUnit',
  Currency = 'currency',
  Tag = 'tag',
  BillingMode = 'billingMode',
  PricingMethod = 'pricingMethod',
  ForecastMethod = 'forecastMethod',
  PriceList = 'priceList',
}

enum PriceListFormKey {
  StartsOn = 'startsOn',
  Rows = 'rows',
}

@Component({
  selector: 'cc-offer-creation-form',
  templateUrl: './offer-creation-form.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OfferCreationFormComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: OfferCreationFormComponent,
    },
  ],
})
export class OfferCreationFormComponent implements OnInit, OnDestroy, ControlValueAccessor, Validator {
  public formGroup: FormGroup;
  public offerFormKey = OfferFormKey;
  public priceListFormKey = PriceListFormKey;
  public granularity = ELuDateGranularity;
  public formMode = SelectDisplayMode.Form;

  private destroy$: Subject<void> = new Subject<void>();

  constructor() {
  }

  public ngOnInit(): void {
    this.formGroup = new FormGroup({
      [OfferFormKey.Name]: new FormControl(),
      [OfferFormKey.Product]: new FormControl(),
      [OfferFormKey.BillingUnit]: new FormControl(),
      [OfferFormKey.Currency]: new FormControl(),
      [OfferFormKey.Tag]: new FormControl(),
      [OfferFormKey.BillingMode]: new FormControl(),
      [OfferFormKey.PricingMethod]: new FormControl(),
      [OfferFormKey.ForecastMethod]: new FormControl(),
      [OfferFormKey.PriceList]: new FormGroup({
        [PriceListFormKey.StartsOn]: new FormControl({ value: PriceListsTimelineService.defaultStartsOn, disabled: true }),
        [PriceListFormKey.Rows]: new FormControl(),
      }),
    });

    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(form => this.onChange(this.formGroup.getRawValue()));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (form: IOfferCreationForm) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(form: IOfferCreationForm): void {
    if (!!form && this.formGroup.value !== form) {
      this.formGroup.patchValue(form);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formGroup.invalid) {
      return { invalid: true };
    }
  }
}
