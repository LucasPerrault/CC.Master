import { ChangeDetectionStrategy, Component, forwardRef, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, FormGroup, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { IOfferFiltersForm, OfferFilterKey } from '../../models/offer-filters-form.interface';

@Component({
  selector: 'cc-offer-filters',
  templateUrl: './offer-filters.component.html',
  styleUrls: ['./offer-filters.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OfferFiltersComponent),
      multi: true,
    },
  ],
})
export class OfferFiltersComponent implements OnInit, OnDestroy, ControlValueAccessor {
  public get selectedAdditionalFiltersCount(): number {
    return this.additionalFiltersKey.filter(key => this.isSelected(this.formGroup, key)).length;
  }

  public formGroup: FormGroup;
  public formKey = OfferFilterKey;

  public showAdditionalFilters = false;

  private readonly additionalFiltersKey = [OfferFilterKey.Product, OfferFilterKey.Currencies, OfferFilterKey.BillingModes];

  private destroy$: Subject<void> = new Subject<void>();

  constructor() {
    this.formGroup = new FormGroup({
      [OfferFilterKey.Search]: new FormControl(),
      [OfferFilterKey.Tag]: new FormControl(),
      [OfferFilterKey.Product]: new FormControl(),
      [OfferFilterKey.Currencies]: new FormControl(),
      [OfferFilterKey.BillingModes]: new FormControl(),
      [OfferFilterKey.State]: new FormControl(),
    });
  }

  public ngOnInit(): void {
    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(form => this.onChange(form));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (form: IOfferFiltersForm) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(form: IOfferFiltersForm): void {
    if (!!form && this.formGroup.value !== form) {
      this.formGroup.patchValue(form);
    }
  }

  public toggleMoreFiltersDisplay(): void {
    this.showAdditionalFilters = !this.showAdditionalFilters;
  }

  private isSelected(formGroup: FormGroup, key: OfferFilterKey): boolean {
    const formControl = formGroup.get(key);
    return Array.isArray(formControl.value) ? !!formControl.value.length : !!formControl.value;
  }
}
