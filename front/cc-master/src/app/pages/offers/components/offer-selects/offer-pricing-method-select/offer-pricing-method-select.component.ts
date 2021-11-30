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

import { PricingMethod, pricingMethods } from '../../../enums/pricing-method.enum';


@Component({
  selector: 'cc-offer-pricing-method-select',
  templateUrl: './offer-pricing-method-select.component.html',
  styleUrls: ['offer-pricing-method-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OfferPricingMethodSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: OfferPricingMethodSelectComponent,
    },
  ],
})
export class OfferPricingMethodSelectComponent implements OnInit, OnDestroy, ControlValueAccessor, Validator {
  @Input() public required = false;
  @Input() public placeholder: string;
  @Input() public hideClearer = false;
  @Input() public set disabled(isDisabled: boolean) { this.setDisabledState(isDisabled); }

  public pricingMethods = pricingMethods;

  public formControl: FormControl = new FormControl();
  private destroy$: Subject<void> = new Subject<void>();

  constructor() { }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(pricingMethod => this.onChange(pricingMethod));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (pricingMethod: PricingMethod) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(pricingMethod: PricingMethod): void {
    if (!!pricingMethod && this.formControl.value !== pricingMethod) {
      this.formControl.setValue(pricingMethod);
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

  public trackBy(index: number, pricingMethod: string): string {
    return pricingMethod;
  }
}
