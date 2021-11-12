import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { billingUnits, IBillingUnit } from '../../../enums/billing-unit.enum';

@Component({
  selector: 'cc-offer-billing-unit-select',
  templateUrl: './offer-billing-unit-select.component.html',
  styleUrls: ['./offer-billing-unit-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OfferBillingUnitSelectComponent),
      multi: true,
    },
  ],
})
export class OfferBillingUnitSelectComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() public placeholder: string;
  @Input() public required = false;
  @Input() public set disabled(isDisabled: boolean) { this.setDisabledState(isDisabled); }

  public billingUnits: IBillingUnit[];
  public formControl: FormControl = new FormControl();

  private destroy$: Subject<void> = new Subject<void>();

  constructor(private translatePipe: TranslatePipe) {
    this.billingUnits = billingUnits.map(b => this.getTranslatedBillingUnit(b));
  }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(billingUnit => this.onChange(billingUnit));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (billingUnit: IBillingUnit) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(billingUnit: IBillingUnit): void {
    if (!!billingUnit && this.formControl.value !== billingUnit) {
      this.formControl.setValue(this.getTranslatedBillingUnit(billingUnit));
    }
  }

  public setDisabledState(isDisabled: boolean) {
    if (isDisabled) {
      this.formControl.disable();
      return;
    }
    this.formControl.enable();
  }

  public trackBy(index: number, billingUnit: IBillingUnit): number {
    return billingUnit.id;
  }

  private getTranslatedBillingUnit(unit: IBillingUnit): IBillingUnit {
    return { ...unit, name: this.translatePipe.transform(unit.name) };
  }
}
