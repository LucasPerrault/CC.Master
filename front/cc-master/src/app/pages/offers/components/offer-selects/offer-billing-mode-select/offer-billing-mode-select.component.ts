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
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: OfferBillingModeSelectComponent,
    },
  ],
})
export class OfferBillingModeSelectComponent implements OnInit, OnDestroy, ControlValueAccessor, Validator {
  @Input() public required = false;
  @Input() public placeholder: string;
  @Input() public multiple = false;
  @Input() public hideClearer = false;
  @Input() public set disabled(isDisabled: boolean) { this.setDisabledState(isDisabled); }

  public billingModes: IBillingMode[];
  public formControl: FormControl = new FormControl();

  private destroy$: Subject<void> = new Subject<void>();

  constructor(private translatePipe: TranslatePipe) {
    this.billingModes = billingModes.map(b => this.getTranslatedBillingMode(b));
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

  public onChange: (billingMode: IBillingMode | IBillingMode[]) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(billingMode: IBillingMode | IBillingMode[]): void {
    if (!!billingMode && this.formControl.value !== billingMode) {
      this.formControl.setValue(this.getTranslatedBillingModes(billingMode));
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formControl.invalid) {
      return { invalid: true };
    }
  }

  public trackBy(index: number, billingMode: IBillingMode): string {
    return billingMode.id;
  }

  public setDisabledState(isDisabled: boolean) {
    if (isDisabled) {
      this.formControl.disable();
      return;
    }
    this.formControl.enable();
  }

  private getTranslatedBillingModes(mode: IBillingMode | IBillingMode[]): IBillingMode | IBillingMode[] {
    return Array.isArray(mode)
      ? mode.map(m => this.getTranslatedBillingMode(m))
      : this.getTranslatedBillingMode(mode);
  }

  private getTranslatedBillingMode(mode: IBillingMode): IBillingMode {
    return { ...mode, name: this.translatePipe.transform(mode.name) };
  }
}
