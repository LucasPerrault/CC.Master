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
import { IOffer } from '@cc/domain/billing/offers';
import { BehaviorSubject, Subject } from 'rxjs';
import { filter, finalize, switchMap, takeUntil, tap } from 'rxjs/operators';

import { OffersDataService } from '../../services/offers-data.service';
import { IOfferForm } from './offer-form.interface';

enum OfferFormKey {
  Name = 'name',
  Product = 'product',
  BillingUnit = 'billingUnit',
  Currency = 'currency',
  SageBusiness = 'sageBusiness',
  Tag = 'tag',
  BillingMode = 'billingMode',
  PricingMethod = 'pricingMethod',
  ForecastMethod = 'forecastMethod',
  PriceList = 'priceLists',
}

@Component({
  selector: 'cc-offer-form',
  templateUrl: './offer-form.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OfferFormComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: OfferFormComponent,
    },
  ],
})
export class OfferFormComponent implements OnInit, OnDestroy, ControlValueAccessor, Validator {
  public formGroup: FormGroup;
  public formKey = OfferFormKey;
  public formMode = SelectDisplayMode.Form;

  public offer: FormControl = new FormControl();
  public isPriceListsLoading$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  private destroy$: Subject<void> = new Subject<void>();

  constructor(private offersDataService: OffersDataService) {
    this.formGroup = new FormGroup({
      [OfferFormKey.Name]: new FormControl(),
      [OfferFormKey.Product]: new FormControl(),
      [OfferFormKey.BillingUnit]: new FormControl(),
      [OfferFormKey.Currency]: new FormControl(),
      [OfferFormKey.SageBusiness]: new FormControl(),
      [OfferFormKey.Tag]: new FormControl(),
      [OfferFormKey.BillingMode]: new FormControl(),
      [OfferFormKey.PricingMethod]: new FormControl(),
      [OfferFormKey.ForecastMethod]: new FormControl(),
      [OfferFormKey.PriceList]: new FormControl(),
    });
  }

  public ngOnInit(): void {
    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(form => this.onChange(form));

    this.offer.valueChanges
      .pipe(
        takeUntil(this.destroy$), filter(o => !!o),
        tap(() => this.isPriceListsLoading$.next(true)),
        switchMap((o: IOffer) => this.offersDataService.getPriceLists$(o.id)
          .pipe(finalize(() => this.isPriceListsLoading$.next(false)))),
      )
      .subscribe(priceLists => this.formGroup.get(OfferFormKey.PriceList).setValue(priceLists));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (form: IOfferForm) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(form: IOfferForm): void {
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
