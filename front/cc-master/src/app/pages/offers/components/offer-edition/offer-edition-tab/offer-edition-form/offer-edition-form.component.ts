import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
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

import { IOfferValidationContext } from '../../../../models/offer-validation-context.interface';
import { OfferRestrictionsService } from '../../../../services/offer-restrictions.service';
import { IOfferEditionForm } from './offer-edition-form.interface';

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
  PriceList = 'priceList',
}

enum PriceListFormKey {
  StartsOn = 'startsOn',
  Rows = 'rows',
}

@Component({
  selector: 'cc-offer-edition-form',
  templateUrl: './offer-edition-form.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OfferEditionFormComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: OfferEditionFormComponent,
    },
  ],
})
export class OfferEditionFormComponent implements OnInit, OnDestroy, ControlValueAccessor, Validator {
  @Input() public set validationContext(context: IOfferValidationContext) {
    this.setFormGroupValidation(context);
  }

  public formGroup: FormGroup;
  public offerFormKey = OfferFormKey;
  public priceListFormKey = PriceListFormKey;
  public granularity = ELuDateGranularity;
  public formMode = SelectDisplayMode.Form;

  private destroy$: Subject<void> = new Subject<void>();

  constructor(private restrictionsService: OfferRestrictionsService) {
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
      [OfferFormKey.PriceList]: new FormGroup({
        [PriceListFormKey.StartsOn]: new FormControl(),
        [PriceListFormKey.Rows]: new FormControl(),
      }),
    });
  }

  public ngOnInit(): void {
    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.onChange(this.formGroup.getRawValue()));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (form: IOfferEditionForm) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(form: IOfferEditionForm): void {
    if (!!form && this.formGroup.value !== form) {
      this.formGroup.patchValue(form);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formGroup.invalid) {
      return { invalid: true };
    }
  }

  private setFormGroupValidation(context: IOfferValidationContext) {
    if (!this.restrictionsService.canEdit(context)) {
      this.formGroup.disable();
      this.formGroup.get(OfferFormKey.Name).enable();
    }

    if (!this.restrictionsService.canEditPriceListStartsOn(context)) {
      this.formGroup.get(OfferFormKey.PriceList).get(PriceListFormKey.StartsOn).disable();
    }
  }
}
