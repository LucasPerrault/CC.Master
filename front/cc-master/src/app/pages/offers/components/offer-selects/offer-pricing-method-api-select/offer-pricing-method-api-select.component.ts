import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';


@Component({
  selector: 'cc-offer-pricing-method-api-select',
  templateUrl: './offer-pricing-method-api-select.component.html',
  styleUrls: ['offer-pricing-method-api-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OfferPricingMethodApiSelectComponent),
      multi: true,
    },
  ],
})
export class OfferPricingMethodApiSelectComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() public required = false;
  @Input() public placeholder: string;
  @Input() public set disabled(isDisabled: boolean) { this.setDisabledState(isDisabled); }

  public api = '/api/v3/offers/pricingMethod';

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

  public onChange: (pricingMethod: string) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(pricingMethod: string): void {
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

  public trackBy(index: number, pricingMethod: string): string {
    return pricingMethod;
  }
}
