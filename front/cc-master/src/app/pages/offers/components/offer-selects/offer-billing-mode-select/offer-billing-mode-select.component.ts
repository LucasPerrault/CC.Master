import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { billingModes, IBillingMode } from '../../../enums/billing-mode.enum';

@Component({
  selector: 'cc-offer-billing-mode-select',
  templateUrl: './offer-billing-mode-select.component.html',
  styleUrls: ['./offer-billing-mode-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OfferBillingModeSelectComponent),
      multi: true,
    },
  ],
})
export class OfferBillingModeSelectComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() public placeholder: string;
  public billingModes: IBillingMode[];
  public formControl: FormControl = new FormControl();

  private destroy$: Subject<void> = new Subject<void>();

  constructor(private translatePipe: TranslatePipe) {
    this.billingModes = this.getTranslatedBillingModes(billingModes);
  }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(billingMode => this.onChange(billingMode));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (billingMode: IBillingMode) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(billingMode: IBillingMode): void {
    if (!!billingMode && this.formControl.value !== billingMode) {
      this.formControl.setValue(billingMode);
    }
  }

  public trackBy(index: number, billingMode: IBillingMode): number {
    return billingMode.id;
  }

  private getTranslatedBillingModes(modes: IBillingMode[]) {
    return modes.map(b => ({ ...b, name: this.translatePipe.transform(b.name) }));
  }
}
